import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../api/AxiosClient";
import { Link } from "react-router-dom";

function Login({ setRole }) {
  const [form, setForm] = useState({ username: "", password: "" });
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await api.post("/api/Auth/login", form);
      localStorage.setItem("token", response.data.token);

      const profile = await api.get("/api/Auth/profile");
      const role = profile.data.role;
      setRole(role);

      if (role === "Admin") {
        navigate("/admin");
      } else {
        navigate("/user");
      }
    } catch (err) {
      localStorage.removeItem("token");
      alert("Login mislukt. Controleer je gebruikersnaam of wachtwoord.");
    }
  };

  return (
    
    <form onSubmit={handleSubmit}>
      <input
        placeholder="Gebruikersnaam"
        onChange={(e) => setForm({ ...form, username: e.target.value })}
      />
      <input
        placeholder="Wachtwoord"
        type="password"
        onChange={(e) => setForm({ ...form, password: e.target.value })}
      />
      <button type="submit">Login</button>
      <p>
        Nog geen account? <Link to="/register">Registreer hier</Link>
      </p>
    </form>
  );
}

export default Login;
