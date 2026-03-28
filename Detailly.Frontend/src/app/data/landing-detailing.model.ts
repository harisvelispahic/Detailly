/**
 * Landing page models for Detailing service
 */

export interface Feature {
  icon: string; // material icon name
  title: string;
  description: string;
}

export interface Service {
  id: number;
  name: string;
  description: string;
  price: number;
  image: string;
  duration: string;
  popular: boolean;
}

export interface PricingTier {
  name: string;
  price: number;
  description: string;
  features: string[];
  popular: boolean;
}

export interface Review {
  id: number;
  rating: number; // 1-5
  comment: string;
  userName: string;
  userAvatar: string;
  serviceName: string;
}

export interface LandingStats {
  carsDetailed: string;
  averageRating: string;
  yearsExperience: string;
}
