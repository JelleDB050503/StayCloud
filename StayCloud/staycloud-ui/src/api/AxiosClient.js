import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5140', 
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
