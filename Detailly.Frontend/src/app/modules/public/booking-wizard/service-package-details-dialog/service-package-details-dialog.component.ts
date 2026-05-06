import { Component, Inject, OnInit, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ServicePackagesApiService } from '../../../../api-services/service-packages/service-packages-api.service';
import {
  GetServicePackageByIdQueryDto,
  ListServicePackagesQueryDto,
  ServicePackageImageDto,
} from '../../../../api-services/service-packages/service-packages-api.models';

export interface ServicePackageDetailsDialogData {
  packageId: number;
}

@Component({
  selector: 'app-service-package-details-dialog',
  standalone: false,
  templateUrl: './service-package-details-dialog.component.html',
  styleUrl: './service-package-details-dialog.component.scss',
})
export class ServicePackageDetailsDialogComponent implements OnInit {
  private api = inject(ServicePackagesApiService);
  private dialogRef = inject(MatDialogRef<ServicePackageDetailsDialogComponent>);

  package: GetServicePackageByIdQueryDto | null = null;
  isLoading = true;

  // Gallery lightbox
  lightboxImage: ServicePackageImageDto | null = null;

  constructor(@Inject(MAT_DIALOG_DATA) public data: ServicePackageDetailsDialogData) {}

  ngOnInit(): void {
    this.api.getById(this.data.packageId).subscribe({
      next: (pkg) => {
        this.package = pkg;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  get galleryImages(): ServicePackageImageDto[] {
    return this.package?.images ?? [];
  }

  get thumbnail(): ServicePackageImageDto | undefined {
    const imgs = this.galleryImages;
    return imgs.find((i) => i.isThumbnail) ?? imgs[0];
  }

  openLightbox(img: ServicePackageImageDto): void {
    this.lightboxImage = img;
  }

  closeLightbox(): void {
    this.lightboxImage = null;
  }

  lightboxPrev(): void {
    if (!this.lightboxImage || !this.galleryImages.length) return;
    const idx = this.galleryImages.findIndex((i) => i.id === this.lightboxImage!.id);
    this.lightboxImage = this.galleryImages[(idx - 1 + this.galleryImages.length) % this.galleryImages.length];
  }

  lightboxNext(): void {
    if (!this.lightboxImage || !this.galleryImages.length) return;
    const idx = this.galleryImages.findIndex((i) => i.id === this.lightboxImage!.id);
    this.lightboxImage = this.galleryImages[(idx + 1) % this.galleryImages.length];
  }

  downloadUrl(img: ServicePackageImageDto): string {
    return this.api.getDownloadUrl(this.data.packageId, img.id);
  }

  close(): void {
    this.dialogRef.close();
  }
}
