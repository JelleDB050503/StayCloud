import axios from 'axios';

const api = axios.create({
  baseURL: 'https://staycloud-booking-jdb.azurewebsites.net', 
});

// ✅ Token toevoegen
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`; // Zorg dat het exact zo is
  }
  return config;
});

export default api;
