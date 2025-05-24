import { Routes, Route, Link } from "react-router-dom";
import CreateBookingForm from "./CreateBookingForm";
import MyBookings from "./MyBookings";
import Caravan1Info from "./Caravan1Info";
import ChaletInfo from "./ChaletInfo";
import Caravan2Info from "./Caravan2Info";
import GeneralLocation from "./GeneralLocation.jsx";
import GeneralPrices from "./GeneralPrices.jsx";
import LogoutButton from "../components/LogoutButton";

function UserDashboard({ setRole, user }) {
  return (
    <div style={{ padding: "1rem" }}>
      <h2>Welkom {user?.username || "Gebruiker"}</h2>
      <nav style={{ marginBottom: "1rem" }}>
        <Link to="/user/create">Boeking Aanvragen</Link> |{" "}
        <Link to="/user/my-bookings">Mijn Boekingen</Link> |{" "}
        <Link to="/user/location">Ligging</Link> |{" "}
        <Link to="/user/prices">Prijzen</Link> |{" "}
        <Link to="/user/caravan1">Caravan 1</Link> |{" "}
        <Link to="/user/caravan2">Caravan 2</Link> |{" "}
        <Link to="/user/chalet">Chalet</Link> |{" "}
        <LogoutButton setRole={setRole} />
      </nav>

      <Routes>
        <Route path="create" element={<CreateBookingForm />} />
        <Route path="my-bookings" element={<MyBookings />} />
        <Route path="location" element={<GeneralLocation />} />
        <Route path="prices" element={<GeneralPrices />} />
        <Route path="caravan1" element={<Caravan1Info />} />
        <Route path="caravan2" element={<Caravan2Info />} />
        <Route path="chalet" element={<ChaletInfo />} />
      </Routes>
      Wij verhuren 3 vakantiewoningen in de Ardennen (Hotton). <br /> 
      Meer informatie kunt u vinden als u in de navigatiebalk hierboven klikt op de namen van de vakantiewoning. <br />
      Moest u nog vragen hebben, kunt u mij contacteren op het nummer:
    </div>
  );
}

export default UserDashboard;
