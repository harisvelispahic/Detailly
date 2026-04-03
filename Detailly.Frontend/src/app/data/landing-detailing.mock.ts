/**
 * Mock data for Detailing service landing page
 * TODO: Replace with actual API calls later
 */

import { Feature, Service, PricingTier, Review, LandingStats } from './landing-detailing.model';

export const mockLandingStats: LandingStats = {
  carsDetailed: '5,000+',
  averageRating: '4.9',
  yearsExperience: '10+',
};

export const mockFeatures: Feature[] = [
  {
    icon: 'sparkles',
    title: 'Premium Products',
    description: 'We use only the highest quality detailing products for your vehicle.',
  },
  {
    icon: 'shield',
    title: 'Paint Protection',
    description: 'Advanced ceramic coatings and sealants for long-lasting protection.',
  },
  {
    icon: 'schedule',
    title: 'Convenient Booking',
    description: 'Easy online scheduling with flexible time slots that fit your life.',
  },
  {
    icon: 'verified_user',
    title: 'Expert Team',
    description: 'Certified detailers with years of experience in vehicle care.',
  },
];

export const mockServices: Service[] = [
  {
    id: 1,
    name: 'Essential Wash',
    description: 'Complete exterior wash with attention to detail and shine',
    price: 49,
    image: 'https://4kwallpapers.com/images/walls/thumbs_3t/21331.jpg',
    duration: '1-2 hours',
    popular: false,
  },
  {
    id: 2,
    name: 'Premium Detail',
    description: 'Interior and exterior complete detailing with ceramic protection',
    price: 149,
    image:
      'https://cdn.displate.com/artwork/857x1200/2024-07-16/732d168b68ac125bf61f249d9a0838f2_c78aca96b0bf5df61c726e0c5b887b95.jpg',
    duration: '3-4 hours',
    popular: true,
  },
  {
    id: 3,
    name: 'Paint Correction',
    description: 'Advanced paint correction and ceramic sealant application',
    price: 299,
    image: 'https://i.pinimg.com/736x/3a/33/39/3a3339fb3dcc945116fe3731411a9863.jpg',
    duration: '4-5 hours',
    popular: false,
  },
  {
    id: 4,
    name: 'Ultimate Showroom',
    description: 'Complete transformation with all services and 90-day protection',
    price: 399,
    image: 'https://i.imgur.com/RAwZLAf.png',
    duration: '6-8 hours',
    popular: false,
  },
];

export const mockPricingTiers: PricingTier[] = [
  {
    name: 'Essential',
    price: 49,
    description: 'Perfect for regular maintenance',
    features: ['Exterior hand wash', 'Tire & wheel cleaning', 'Window cleaning', 'Air freshener'],
    popular: false,
  },
  {
    name: 'Premium',
    price: 149,
    description: 'Complete interior & exterior care',
    features: [
      'Everything in Essential',
      'Full interior vacuum',
      'Dashboard & console detail',
      'Leather conditioning',
      'Odor elimination',
    ],
    popular: true,
  },
  {
    name: 'Ultimate',
    price: 399,
    description: 'The full showroom experience',
    features: [
      'Everything in Premium',
      'Clay bar treatment',
      'Paint correction',
      'Ceramic sealant',
      '90-day protection',
    ],
    popular: false,
  },
];

export const mockReviews: Review[] = [
  {
    id: 1,
    rating: 5,
    comment:
      'Detailly transformed my car! The attention to detail is incredible. Every surface shines and smells fresh. Highly recommend!',
    userName: 'Sarah Johnson',
    userAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Sarah',
    serviceName: 'Premium Detail',
  },
  {
    id: 2,
    rating: 5,
    comment:
      "Best detailing service I've ever used. Professional, affordable, and they really care about your vehicle. Will definitely book again!",
    userName: 'Michael Chen',
    userAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Michael',
    serviceName: 'Ultimate Showroom',
  },
  {
    id: 3,
    rating: 4,
    comment:
      'Great service and friendly team. My car looks brand new! Minor: wait time was a bit longer than expected, but worth it.',
    userName: 'Emma Williams',
    userAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Emma',
    serviceName: 'Paint Correction',
  },
  {
    id: 4,
    rating: 5,
    comment:
      'The ceramic coating has held up perfectly after 3 months. Paint is still glossy and water beads off like nothing. Amazing!',
    userName: 'David Martinez',
    userAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=David',
    serviceName: 'Premium Detail',
  },
  {
    id: 5,
    rating: 5,
    comment:
      'Truly professional team. They explained every step and used premium products. My car looks like it just rolled off the lot.',
    userName: 'Jessica Lee',
    userAvatar: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Jessica',
    serviceName: 'Ultimate Showroom',
  },
];
