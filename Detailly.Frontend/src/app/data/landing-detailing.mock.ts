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
    image: 'https://images.unsplash.com/photo-1552820728-8ac41f1ce891?w=500&h=400&fit=crop',
    duration: '1-2 hours',
    popular: false,
  },
  {
    id: 2,
    name: 'Premium Detail',
    description: 'Interior and exterior complete detailing with ceramic protection',
    price: 149,
    image: 'https://images.unsplash.com/photo-1619405399517-d4dc5ebe6c2b?w=500&h=400&fit=crop',
    duration: '3-4 hours',
    popular: true,
  },
  {
    id: 3,
    name: 'Paint Correction',
    description: 'Advanced paint correction and ceramic sealant application',
    price: 299,
    image: 'https://images.unsplash.com/photo-1615906655593-628e95dab11c?w=500&h=400&fit=crop',
    duration: '4-5 hours',
    popular: false,
  },
  {
    id: 4,
    name: 'Ultimate Showroom',
    description: 'Complete transformation with all services and 90-day protection',
    price: 399,
    image: 'https://images.unsplash.com/photo-1489824904134-891ab64532f1?w=500&h=400&fit=crop',
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
