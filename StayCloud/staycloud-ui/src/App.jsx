import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
} from "react-router-dom";
import Login from "./auth/Login";
import Register from "./auth/Register";
import UserDashboard from "./user/UserDashboard";
import AdminDashboard from "./admin/AdminDashboard";
import { useEffect, useState } from "react";
import api from "./api/AxiosClient";
import './style.css'

function App() {
  const [role, setRole] = useState(null);
  const [user, setUser] = useState(null);

  useEffect(() => {
    const token = localStorage.getItem("token");

    if (!token) {
      setRole(null); // Geen login = naar loginpagina
      return;
    }

    const fetchProfile = async () => {
      try {
        const res = await api.get("/api/Auth/profile");
        setRole(res.data.role);
        setUser(res.data)
      } catch (err) {
        // Alleen op Unauthorized de role resetten
        if (err.response?.status === 401) {
          setRole(null);
        } else {
          console.error("Fout bij ophalen profiel:", err);
        }
      }
    };

    fetchProfile();
  }, []);

  return (
    <Router>
      <Routes>
        {role === null ? (
          <>
            <Route path="/login" element={<Login setRole={setRole} />} />
            <Route path="/register" element={<Register />} />
            <Route path="*" element={<Navigate to="/login" />} />
          </>
        ) : role === "admin" ? (
          <>
            <Route
              path="/admin/*"
              element={<AdminDashboard setRole={setRole} user={user} />}
            />
            <Route path="*" element={<Navigate to="/admin" />} />
          </>
        ) : (
          <>
            <Route
              path="/user/*"
              element={<UserDashboard setRole={setRole} user={user} />}
            />
            <Route path="*" element={<Navigate to="/user" />} />
          </>
        )}
      </Routes>
    </Router>
  );
}


export default App;
