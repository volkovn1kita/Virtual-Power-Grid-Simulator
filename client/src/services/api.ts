import axios from 'axios';
import type { GridSnapshot, PowerPlant, PowerConsumer } from '../types/models';

// Перевір, чи порт правильний (з Swagger)
const API_URL = 'http://localhost:5250/api'; 

export const powerGridApi = {
    // 1. Отримати загальний стан (Частота, Суми)
    getGridStatus: async (): Promise<GridSnapshot> => {
        const response = await axios.get<GridSnapshot>(`${API_URL}/PowerGrid/status`);
        return response.data;
    },

    // 2. Отримати список станцій
    getPlants: async (): Promise<PowerPlant[]> => {
        const response = await axios.get<PowerPlant[]>(`${API_URL}/PowerGrid/plants`);
        return response.data;
    },

    // 3. Отримати список споживачів
    getConsumers: async (): Promise<PowerConsumer[]> => {
        const response = await axios.get<PowerConsumer[]>(`${API_URL}/PowerGrid/consumers`);
        return response.data;
    },

    turnOnPlant: async (id: string) => {
        await axios.post(`${API_URL}/PowerGrid/plants/${id}/turn-on`);
    },

    turnOffPlant: async (id: string) => {
        await axios.post(`${API_URL}/PowerGrid/plants/${id}/turn-off`);
    },

    adjustPower: async (id: string, targetPower: number) => {
        await axios.post(`${API_URL}/PowerGrid/plants/${id}/adjust`, { targetPower });
    }
};