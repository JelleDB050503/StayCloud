import React, {useEffect, useState} from "react";
import api from "../api/AxiosClient";

function BookingList(){
    const [bookings, setBookings] = useState([]);

    useEffect(() => {
        async function fetchBookings(){
            const res = await api.get('/booking/all');
            setBookings(res.data);
        }
        fetchBookings();
    }, []);

    return (
        <div>
            <h2>Alle boekingen</h2>
            <ul>
                {bookings.map(b => (
                    <li key={b.id}>
                        {b.guestname} - {b.accommodationType} - GoedGekeurd = {b.isApproved ? 'Ja': 'Nee'}
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default BookingList;