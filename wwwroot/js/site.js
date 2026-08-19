// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener("DOMContentLoaded", function () {
    const searchForm = document.getElementById("flightSearchForm");
    
    if (searchForm) {
        searchForm.addEventListener("submit", function (e) {
            e.preventDefault();
            searchFlights();
        });
    }
});

function searchFlights() {
    const depIata = document.getElementById("depIata").value.trim();
    const arrIata = document.getElementById("arrIata").value.trim();
    const airlineName = document.getElementById("airlineName") ? document.getElementById("airlineName").value.trim() : "";
    
    if (!depIata) {
        showError("Kode IATA keberangkatan wajib diisi.");
        return;
    }

    const loadingIndicator = document.getElementById("loadingIndicator");
    const resultsContainer = document.getElementById("flightResults");
    const errorMessage = document.getElementById("errorMessage");
    const btnSearch = document.getElementById("btnSearch");

    // Reset UI state
    errorMessage.classList.add("d-none");
    resultsContainer.innerHTML = "";
    loadingIndicator.classList.remove("d-none");
    btnSearch.disabled = true;

    // Build URL
    let url = `/Home/SearchFlights?depIata=${encodeURIComponent(depIata)}`;
    if (arrIata) {
        url += `&arrIata=${encodeURIComponent(arrIata)}`;
    }
    if (airlineName) {
        url += `&airlineName=${encodeURIComponent(airlineName)}`;
    }

    fetch(url)
        .then(response => {
            if (!response.ok) {
                throw new Error("Terjadi kesalahan saat mengambil data penerbangan.");
            }
            return response.json();
        })
        .then(data => {
            loadingIndicator.classList.add("d-none");
            btnSearch.disabled = false;
            
            if (!data || data.length === 0) {
                showError("Tidak ada jadwal penerbangan yang ditemukan untuk rute ini.");
                return;
            }

            renderFlights(data);
        })
        .catch(error => {
            loadingIndicator.classList.add("d-none");
            btnSearch.disabled = false;
            showError(error.message);
        });
}

function renderFlights(flights) {
    const resultsContainer = document.getElementById("flightResults");
    let html = "";

    flights.forEach(flight => {
        const flightNumber = flight.flight?.iata || flight.flight?.number || "N/A";
        const airlineName = flight.airline?.name || "Unknown Airline";
        const status = flight.flight_status || "unknown";
        
        // Format dates
        const depTime = flight.departure?.scheduled ? new Date(flight.departure.scheduled).toLocaleString('id-ID') : "TBA";
        const arrTime = flight.arrival?.scheduled ? new Date(flight.arrival.scheduled).toLocaleString('id-ID') : "TBA";
        
        // Badge color based on status
        let statusBadge = "bg-secondary";
        if (status === "active") statusBadge = "bg-success";
        else if (status === "scheduled") statusBadge = "bg-primary";
        else if (status === "landed") statusBadge = "bg-info";
        else if (status === "cancelled") statusBadge = "bg-danger";

        html += `
        <div class="col-md-6 col-lg-4 mb-4">
            <div class="card h-100 shadow-sm border-0 flight-card">
                <div class="card-header bg-white border-bottom-0 pt-4 pb-0 d-flex justify-content-between align-items-center">
                    <h5 class="mb-0 text-primary fw-bold">${airlineName}</h5>
                    <span class="badge ${statusBadge} rounded-pill text-uppercase">${status}</span>
                </div>
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center mb-3">
                        <div class="text-center">
                            <h3 class="fw-bold mb-0">${flight.departure?.iata || "-"}</h3>
                            <small class="text-muted">${flight.departure?.terminal ? 'T' + flight.departure.terminal : ''} ${flight.departure?.gate ? 'G' + flight.departure.gate : ''}</small>
                        </div>
                        <div class="text-center text-muted px-2">
                            <i class="fas fa-plane text-primary fs-4"></i><br>
                            <small class="fw-bold">${flightNumber}</small>
                        </div>
                        <div class="text-center">
                            <h3 class="fw-bold mb-0">${flight.arrival?.iata || "-"}</h3>
                            <small class="text-muted">${flight.arrival?.terminal ? 'T' + flight.arrival.terminal : ''} ${flight.arrival?.gate ? 'G' + flight.arrival.gate : ''}</small>
                        </div>
                    </div>
                    
                    <ul class="list-group list-group-flush small">
                        <li class="list-group-item px-0 d-flex justify-content-between">
                            <span class="text-muted"><i class="far fa-calendar-alt me-2"></i>Keberangkatan:</span>
                            <strong>${depTime}</strong>
                        </li>
                        <li class="list-group-item px-0 d-flex justify-content-between">
                            <span class="text-muted"><i class="far fa-clock me-2"></i>Kedatangan:</span>
                            <strong>${arrTime}</strong>
                        </li>
                    </ul>
                </div>
            </div>
        </div>`;
    });

    resultsContainer.innerHTML = html;
}

function showError(message) {
    const errorMessage = document.getElementById("errorMessage");
    errorMessage.textContent = message;
    errorMessage.classList.remove("d-none");
}
