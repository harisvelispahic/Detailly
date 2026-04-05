import { Component } from '@angular/core';

interface ShopProductCard {
  name: string;
  category: string;
  price: string;
  rating: string;
  accent: string;
}

@Component({
  selector: 'app-search-products',
  standalone: false,
  templateUrl: './search-products.component.html',
  styleUrl: './search-products.component.scss',
})
export class SearchProductsComponent {
  readonly categories = [
    'Wash & Polish',
    'Interior Care',
    'Ceramic',
    'Accessories',
    'Equipment',
  ];

  readonly products: ShopProductCard[] = [
    {
      name: 'Premium Car Wax',
      category: 'Protection',
      price: '$24.99',
      rating: '4.8',
      accent: 'linear-gradient(135deg, hsl(275 77% 61%) 0%, hsl(225 65% 48%) 100%)',
    },
    {
      name: 'Microfiber Towel Set',
      category: 'Accessories',
      price: '$18.50',
      rating: '4.9',
      accent: 'linear-gradient(135deg, hsl(240 16% 22%) 0%, hsl(240 12% 12%) 100%)',
    },
    {
      name: 'Interior Cleaner Kit',
      category: 'Interior Care',
      price: '$39.99',
      rating: '4.7',
      accent: 'linear-gradient(135deg, hsl(32 78% 57%) 0%, hsl(278 70% 56%) 100%)',
    },
    {
      name: 'Foam Cannon Pro',
      category: 'Equipment',
      price: '$89.99',
      rating: '4.9',
      accent: 'linear-gradient(135deg, hsl(12 76% 57%) 0%, hsl(269 72% 57%) 100%)',
    },
    {
      name: 'Ceramic Spray',
      category: 'Ceramic',
      price: '$54.99',
      rating: '4.8',
      accent: 'linear-gradient(135deg, hsl(242 19% 18%) 0%, hsl(248 23% 9%) 100%)',
    },
    {
      name: 'Wheel Cleaner Gel',
      category: 'Exterior',
      price: '$19.99',
      rating: '4.7',
      accent: 'linear-gradient(135deg, hsl(213 70% 54%) 0%, hsl(285 69% 57%) 100%)',
    },
  ];
}
