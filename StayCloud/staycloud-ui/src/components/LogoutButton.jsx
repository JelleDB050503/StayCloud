function LogoutButton() {
  const handleLogout = () => {
    console.log("Uitloggen gestart");
    localStorage.removeItem("token");
    window.location.href = "/login"; // ✅ geforceerde redirect
  };

  return <button onClick={handleLogout}>Uitloggen</button>;
}

export default LogoutButton;
