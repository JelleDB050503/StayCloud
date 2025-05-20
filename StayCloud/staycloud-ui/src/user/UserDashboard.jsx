import { Routes, Route, Link } from "react-router-dom";
import CreateBookingForm from "./CreateBookingForm";
import MyBookings from "./MyBookings";
import LogoutButton from "../components/LogoutButton";

function UserDashboard({ setRole, user }) {
  return (
    <div style={{ padding: "1rem" }}>
      <h2>Welkom {user?.username || "Gebruiker"}</h2>
      <nav style={{ marginBottom: "1rem" }}>
        <Link to="/user/create">Boeking Aanvragen</Link> |{" "}
        <Link to="/user/my-bookings">Mijn Boekingen</Link> |{" "}
        <LogoutButton setRole={setRole} />
      </nav>

      <Routes>
        <Route path="create" element={<CreateBookingForm />} />
        <Route path="my-bookings" element={<MyBookings />} />
      </Routes>
    </div>
  );
}

export default UserDashboard;
