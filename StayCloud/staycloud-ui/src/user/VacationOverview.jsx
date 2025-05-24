import { useState } from "react";
import Caravan1Info from "./Caravan1Info.jsx";
import Caravan2Info from "./Caravan2Info.jsx";
import ChaletInfo from "./ChaletInfo.jsx";
import GeneralPrices from "./GeneralPrices.jsx";
import GeneralLocation from "./GeneralLocation.jsx";

export default function VacationOverview() {
  const [activeTab, setActiveTab] = useState("caravan1");

  return (
    <div className="p-4">
      <h1 className="text-2xl font-bold mb-4">Vakantiewoningen</h1>

      <div className="flex gap-4 mb-4">
        <button onClick={() => setActiveTab("caravan1")}>Caravan 1</button>
        <button onClick={() => setActiveTab("caravan2")}>Caravan 2</button>
        <button onClick={() => setActiveTab("chalet")}>Chalet</button>
        <button onClick={() => setActiveTab("prijzen")}>Prijzen</button>
        <button onClick={() => setActiveTab("ligging")}>Ligging</button>
      </div>

      {activeTab === "caravan1" && <Caravan1Info />}
      {activeTab === "caravan2" && <Caravan2Info />}
      {activeTab === "chalet" && <ChaletInfo />}
      {activeTab === "prijzen" && <GeneralPrices />}
      {activeTab === "ligging" && <GeneralLocation />}
    </div>
  );
}
