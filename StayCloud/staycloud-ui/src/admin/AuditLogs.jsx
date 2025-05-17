import React, { useEffect, useState } from "react";
import api from "../api/AxiosClient";

function AuditLogs() {
  const [logs, setLogs] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchLogs = async () => {
      try {
        const response = await api.get("/api/auth/auditlogs");
        setLogs(response.data);
      } catch (err) {
        console.error("Fout bij ophalen audit logs:", err);
        setError("Audit logs ophalen is mislukt.");
      } finally {
        setLoading(false);
      }
    };

    fetchLogs();
  }, []);

  if (loading) return <p>Audit logs laden...</p>;
  if (error) return <p style={{ color: "red" }}>{error}</p>;

  return (
    <div>
      <h2>Audit Logs</h2>
      {logs.length === 0 ? (
        <p>Geen logs beschikbaar.</p>
      ) : (
        <table border="1" cellPadding="5">
          <thead>
            <tr>
              <th>Actie</th>
              <th>Uitgevoerd door</th>
              <th>Doelgebruiker</th>
              <th>Tijdstip (UTC)</th>
            </tr>
          </thead>
          <tbody>
            {logs.map((log) => (
              <tr key={log.id}>
                <td>{log.action}</td>
                <td>{log.performedBy}</td>
                <td>{log.targetUser}</td>
                <td>{new Date(log.timestamp).toLocaleString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default AuditLogs;
