import React, {useEffect, useState} from "react";
import api from "../api/AxiosClient";

function ApproveBooking(){
    const [bookings, setBookings] = useState([]);

    useEffect(()=>{
        async function fetchBookings() {
            const res = await api.get('/booking/all');
            setBookings(res.data.filter(b => !b.IsApproved));
        }
        fetchBookings();
    }, []);

    const approve = async (id) => {
        await api.post(`/booking/approve/${id}`);
        alert('Boeking goedgekeurd!');
        setBookings(bookings.filter(b => b.id !==id));
    };

    return(
        <div>
            <h2>Te Keuren boekingen</h2>
            <ul>
                {bookings.map(b => (
                    <li key={b.id}>
                        {b.guestname} - {b.accommodationType}
                        <button onClick={()=> approve(b.id)}>Goedkeuren</button>
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default ApproveBooking;