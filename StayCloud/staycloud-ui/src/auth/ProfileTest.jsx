import React from "react";
import api from "../api/AxiosClient";

function ProfileTest() {
  const handleClick = async () => {
    try {
      const res = await api.get("api/Auth/profile");
      alert(res.data); // zou moeten tonen: "Je bent ingelogd als ..."
    } catch (err) {
      console.error("Profiel ophalen faalt:", err);
      alert("Token werkt niet of is verlopen.");
    }
  };

  return <button onClick={handleClick}>Toon profiel (test)</button>;
}

export default ProfileTest;
