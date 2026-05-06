import { Component, Input, OnInit, inject } from '@angular/core';
import { ServicePackagesApiService, ImageUploadProgress } from '../../../../../api-services/service-packages/service-packages-api.service';
import { ServicePackageImageDto } from '../../../../../api-services/service-packages/service-packages-api.models';
import { ToasterService } from '../../../../../core/services/toaster.service';

interface UploadItem {
  file: File;
  preview: string;
  progress: number;
  status: 'uploading' | 'done' | 'error';
  error?: string;
}

@Component({
  selector: 'app-service-package-images',
  standalone: false,
  templateUrl: './service-package-images.component.html',
  styleUrl: './service-package-images.component.scss',
})
export class ServicePackageImagesComponent implements OnInit {
  @Input({ required: true }) packageId!: number;

  private api = inject(ServicePackagesApiService);
  private toaster = inject(ToasterService);

  images: ServicePackageImageDto[] = [];
  uploads: UploadItem[] = [];
  isDragOver = false;
  isLoading = false;

  private readonly MAX_WIDTH = 1200;
  private readonly QUALITY = 0.82;
  private readonly ALLOWED_TYPES = ['image/jpeg', 'image/png', 'image/webp', 'image/gif'];
  private readonly MAX_BYTES = 10 * 1024 * 1024;

  ngOnInit(): void {
    this.loadImages();
  }

  loadImages(): void {
    this.isLoading = true;
    this.api.getById(this.packageId).subscribe({
      next: (pkg) => {
        this.images = pkg.images ?? [];
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    this.isDragOver = true;
  }

  onDragLeave(): void {
    this.isDragOver = false;
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    this.isDragOver = false;
    const files = Array.from(event.dataTransfer?.files ?? []);
    this.processFiles(files);
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const files = Array.from(input.files ?? []);
    this.processFiles(files);
    input.value = '';
  }

  private processFiles(files: File[]): void {
    const imageFiles = files.filter((f) => this.ALLOWED_TYPES.includes(f.type));
    const oversized = files.filter((f) => f.size > this.MAX_BYTES);

    if (oversized.length) {
      this.toaster.error(`${oversized.length} file(s) exceed the 10 MB limit and were skipped.`);
    }

    const valid = imageFiles.filter((f) => f.size <= this.MAX_BYTES);

    const duplicates = valid.filter((f) =>
      this.uploads.some((u) => u.file.name === f.name && u.file.size === f.size),
    );
    if (duplicates.length) {
      this.toaster.error(
        duplicates.length === 1
          ? `"${duplicates[0].name}" is already being uploaded.`
          : `${duplicates.length} file(s) are already being uploaded.`,
      );
    }

    const unique = valid.filter(
      (f) => !this.uploads.some((u) => u.file.name === f.name && u.file.size === f.size),
    );
    unique.forEach((f) => this.compressAndUpload(f));
  }

  private async compressAndUpload(file: File): Promise<void> {
    const preview = await this.toDataUrl(file);

    let compressed: File;
    try {
      const blob = await this.compressImage(file);
      compressed = new File([blob], file.name, { type: blob.type });
    } catch {
      this.toaster.error(`"${file.name}" appears to be corrupted or invalid and cannot be uploaded.`);
      return;
    }

    const item: UploadItem = { file, preview, progress: 0, status: 'uploading' };
    this.uploads.push(item);

    this.api.uploadImage(this.packageId, compressed).subscribe({
      next: (evt: ImageUploadProgress) => {
        item.progress = evt.progress;
        if (evt.done && evt.result) {
          item.status = 'done';
          this.images.push(evt.result);
          setTimeout(() => {
            this.uploads = this.uploads.filter((u) => u !== item);
          }, 1200);
        }
      },
      error: (err) => {
        item.status = 'error';
        item.error = err?.error?.message ?? 'Upload failed.';
      },
    });
  }

  private toDataUrl(file: File): Promise<string> {
    return new Promise((resolve) => {
      const reader = new FileReader();
      reader.onload = (e) => resolve(e.target!.result as string);
      reader.readAsDataURL(file);
    });
  }

  private compressImage(file: File): Promise<Blob> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = (e) => {
        const img = new Image();
        img.onload = () => {
          let { width, height } = img;
          if (width > this.MAX_WIDTH) {
            height = Math.round((height * this.MAX_WIDTH) / width);
            width = this.MAX_WIDTH;
          }
          const canvas = document.createElement('canvas');
          canvas.width = width;
          canvas.height = height;
          canvas.getContext('2d')!.drawImage(img, 0, 0, width, height);
          canvas.toBlob(
            (blob) => (blob ? resolve(blob) : reject()),
            file.type === 'image/png' ? 'image/png' : 'image/jpeg',
            this.QUALITY,
          );
        };
        img.onerror = reject;
        img.src = e.target!.result as string;
      };
      reader.onerror = reject;
      reader.readAsDataURL(file);
    });
  }

  setThumbnail(image: ServicePackageImageDto): void {
    if (image.isThumbnail) return;
    this.api.setThumbnail(this.packageId, image.id).subscribe({
      next: () => {
        this.images.forEach((i) => (i.isThumbnail = i.id === image.id));
        this.toaster.success('Thumbnail updated.');
      },
      error: () => this.toaster.error('Failed to set thumbnail.'),
    });
  }

  deleteImage(image: ServicePackageImageDto): void {
    this.api.deleteImage(this.packageId, image.id).subscribe({
      next: () => {
        this.images = this.images.filter((i) => i.id !== image.id);
        if (image.isThumbnail && this.images.length) {
          this.images[0].isThumbnail = true;
        }
        this.toaster.success('Image deleted.');
      },
      error: () => this.toaster.error('Failed to delete image.'),
    });
  }

  downloadUrl(image: ServicePackageImageDto): string {
    return this.api.getDownloadUrl(this.packageId, image.id);
  }
}
