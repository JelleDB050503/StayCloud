import { Routes, Route, Link } from "react-router-dom";
import AllBookings from "./AllBookings";
import ApproveBookings from "./ApproveBookings";
import CreateBookingAdmin from "./CreateBookingAdmin";
import PromoteUser from "./PromoteUser";
import AuditLogs from "./AuditLogs";
import SearchBooking from "./SearchBooking";
import LogoutButton from "../components/LogoutButton";

function AdminDashboard({ setRole, user }) {
  return (
    <div style={{ padding: "1rem" }}>
      <h2>Welkom {user?.username || "Admin"}</h2>
      <nav style={{ marginBottom: "1rem" }}>
        <Link to="/admin/create">Boeking Aanmaken</Link> |{" "}
        <Link to="/admin/all">Alle Boekingen</Link> |{" "}
        <Link to="/admin/approve">Goedkeuren</Link> |{" "}
        <Link to="/admin/search">Zoeken</Link> |{" "}
        <Link to="/admin/promote">Promoveren</Link> |{" "}
        <Link to="/admin/logs">Audit Logs</Link> |{" "}
        <LogoutButton setRole={setRole} />
      </nav>

      <Routes>
        <Route path="create" element={<CreateBookingAdmin />} />
        <Route path="all" element={<AllBookings />} />
        <Route path="approve" element={<ApproveBookings />} />
        <Route path="search" element={<SearchBooking />} />
        <Route path="promote" element={<PromoteUser />} />
        <Route path="logs" element={<AuditLogs />} />
      </Routes>
    </div>
  );
}

export default AdminDashboard;
