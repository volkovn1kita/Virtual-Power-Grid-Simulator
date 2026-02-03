// PlantType.ts

export type PlantType =
  | 'Thermal'
  | 'Nuclear'
  | 'Hydro'
  | 'Solar'
  | 'Wind';

export const PlantTypeLabels: Record<PlantType, string> = {
  Thermal: 'Thermal',
  Nuclear: 'Nuclear',
  Hydro: 'Hydro',
  Solar: 'Solar',
  Wind: 'Wind',
};


export interface PowerPlant {
    id: string;
    name: string;
    type: number;
    maxCapacity: number;
    currentPower: number;
    isWorking: boolean;
    rampRatePerTick: number;
}

export interface PowerConsumer {
    id: string;
    name: string;
    maxPeakLoad: number;
    priority: number;
    isActive: boolean;
}

// Головний знімок системи, який приходить з бекенду
export interface GridSnapshot {
    timeStamp: string | number | Date;
    timestamp: string;
    plants: PowerPlant[];
    consumers: PowerConsumer[];
    totalGeneration: number;
    totalDemand: number;
    frequency: number; 
}