import React, { useState } from "react";
import api from "../api/AxiosClient";

function PromoteUser() {
  const [username, setUsername] = useState("");
  const [feedback, setFeedback] = useState("");

  const handlePromote = async (e) => {
    e.preventDefault();
    setFeedback("");

    try {
      const response = await api.post("/api/auth/promote", { username });
      setFeedback(response.data);
    } catch (err) {
      console.error("Fout bij promotie:", err);
      setFeedback("Promotie mislukt. Controleer of de gebruikersnaam bestaat.");
    }
  };

  return (
    <div>
      <h2>Gebruiker Promoveren tot Admin</h2>
      <form onSubmit={handlePromote}>
        <label>
          Gebruikersnaam:
          <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
        </label>
        <button type="submit">Promoveren</button>
      </form>
      {feedback && <p style={{ marginTop: "1em" }}>{feedback}</p>}
    </div>
  );
}

export default PromoteUser;
