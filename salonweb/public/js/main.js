/**
 * ================================================
 * SALON BOOKING SYSTEM - MAIN JAVASCRIPT
 * ================================================
 */

// ===== DOM CONTENT LOADED =====
document.addEventListener('DOMContentLoaded', function() {
    // Initialize all components
    initNavigation();
    initCalendar();
    initTimeSlots();
    initServiceSelection();
    initBookingForm();
    initStaffDashboard();
    initAdminDashboard();
    initAnimations();
});

// ===== NAVIGATION =====
function initNavigation() {
    const navbarToggler = document.querySelector('.navbar-toggler');
    const navbarCollapse = document.querySelector('.navbar-collapse');
    
    if (navbarToggler) {
        navbarToggler.addEventListener('click', () => {
            navbarCollapse.classList.toggle('show');
        });
    }
    
    // Active link highlighting
    const currentPage = window.location.pathname.split('/').pop() || 'index.html';
    const navLinks = document.querySelectorAll('.nav-link');
    
    navLinks.forEach(link => {
        const href = link.getAttribute('href');
        if (href === currentPage) {
            link.classList.add('active');
        }
    });
}

// ===== CALENDAR FUNCTIONALITY =====
function initCalendar() {
    const calendar = document.getElementById('bookingCalendar');
    if (!calendar) return;
    
    const currentDate = new Date();
    let selectedDate = null;
    const bookedDates = ['2026-03-05', '2026-03-08', '2026-03-12']; // Demo data
    
    function renderCalendar(month, year) {
        const firstDay = new Date(year, month, 1);
        const lastDay = new Date(year, month + 1, 0);
        const daysInMonth = lastDay.getDate();
        const startingDay = firstDay.getDay();
        
        const monthNames = ['January', 'February', 'March', 'April', 'May', 'June',
                           'July', 'August', 'September', 'October', 'November', 'December'];
        
        let html = `
            <div class="calendar-header d-flex justify-content-between align-items-center mb-3">
                <button class="btn btn-sm btn-outline-secondary" onclick="changeMonth(-1)">
                    <i class="fas fa-chevron-left"></i>
                </button>
                <h5 class="mb-0">${monthNames[month]} ${year}</h5>
                <button class="btn btn-sm btn-outline-secondary" onclick="changeMonth(1)">
                    <i class="fas fa-chevron-right"></i>
                </button>
            </div>
            <div class="calendar-weekdays d-flex flex-wrap">
                <div class="text-center" style="width: 14.28%">Sun</div>
                <div class="text-center" style="width: 14.28%">Mon</div>
                <div class="text-center" style="width: 14.28%">Tue</div>
                <div class="text-center" style="width: 14.28%">Wed</div>
                <div class="text-center" style="width: 14.28%">Thu</div>
                <div class="text-center" style="width: 14.28%">Fri</div>
                <div class="text-center" style="width: 14.28%">Sat</div>
            </div>
            <div class="calendar-days d-flex flex-wrap">
        `;
        
        // Empty cells before first day
        for (let i = 0; i < startingDay; i++) {
            html += '<div class="calendar-day disabled"></div>';
        }
        
        // Days of the month
        for (let day = 1; day <= daysInMonth; day++) {
            const dateStr = `${year}-${String(month + 1).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
            const isToday = day === currentDate.getDate() && month === currentDate.getMonth() && year === currentDate.getFullYear();
            const isPast = new Date(year, month, day) < new Date(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate());
            const isBooked = bookedDates.includes(dateStr);
            const isSelected = selectedDate === dateStr;
            
            let classes = 'calendar-day';
            if (isToday) classes += ' today';
            if (isPast || isBooked) classes += ' disabled';
            if (isSelected) classes += ' selected';
            
            html += `<div class="${classes}" data-date="${dateStr}" onclick="${!isPast && !isBooked ? `selectDate('${dateStr}')` : ''}">${day}</div>`;
        }
        
        html += '</div>';
        calendar.innerHTML = html;
    }
    
    let currentMonth = currentDate.getMonth();
    let currentYear = currentDate.getFullYear();
    
    window.selectDate = function(date) {
        selectedDate = date;
        renderCalendar(currentMonth, currentYear);
        
        // Trigger custom event
        const event = new CustomEvent('dateSelected', { detail: { date: date } });
        document.dispatchEvent(event);
        
        // Show time slots
        showTimeSlots();
    };
    
    window.changeMonth = function(delta) {
        currentMonth += delta;
        if (currentMonth > 11) {
            currentMonth = 0;
            currentYear++;
        } else if (currentMonth < 0) {
            currentMonth = 11;
            currentYear--;
        }
        renderCalendar(currentMonth, currentYear);
    };
    
    renderCalendar(currentMonth, currentYear);
}

// ===== TIME SLOTS =====
function initTimeSlots() {
    const timeSlotsContainer = document.getElementById('timeSlots');
    if (!timeSlotsContainer) return;
    
    window.showTimeSlots = function() {
        const slots = [
            { time: '09:00', available: true },
            { time: '09:30', available: true },
            { time: '10:00', available: false },
            { time: '10:30', available: true },
            { time: '11:00', available: true },
            { time: '11:30', available: true },
            { time: '12:00', available: false },
            { time: '12:30', available: true },
            { time: '13:00', available: true },
            { time: '13:30', available: false },
            { time: '14:00', available: true },
            { time: '14:30', available: true },
            { time: '15:00', available: true },
            { time: '15:30', available: true },
            { time: '16:00', available: false },
            { time: '16:30', available: true },
            { time: '17:00', available: true },
            { time: '17:30', available: false },
            { time: '18:00', available: true }
        ];
        
        let html = '<h6 class="mb-3">Available Time Slots</h6><div class="row g-2">';
        slots.forEach(slot => {
            const classes = `time-slot ${slot.available ? '' : 'unavailable'}`;
            html += `
                <div class="col-4 col-md-3">
                    <div class="${classes}" data-time="${slot.time}" onclick="${slot.available ? `selectTime('${slot.time}')` : ''}">
                        ${slot.time}
                    </div>
                </div>
            `;
        });
        html += '</div>';
        
        timeSlotsContainer.innerHTML = html;
        timeSlotsContainer.classList.add('fade-in');
    };
    
    window.selectTime = function(time) {
        const slots = document.querySelectorAll('.time-slot');
        slots.forEach(slot => slot.classList.remove('selected'));
        
        const selectedSlot = document.querySelector(`.time-slot[data-time="${time}"]`);
        if (selectedSlot) {
            selectedSlot.classList.add('selected');
        }
        
        // Trigger custom event
        const event = new CustomEvent('timeSelected', { detail: { time: time } });
        document.dispatchEvent(event);
    };
}

// ===== SERVICE SELECTION =====
function initServiceSelection() {
    const serviceCards = document.querySelectorAll('.service-card');
    
    serviceCards.forEach(card => {
        card.addEventListener('click', () => {
            const checkbox = card.querySelector('input[type="checkbox"]');
            if (checkbox) {
                checkbox.checked = !checkbox.checked;
            }
            
            card.classList.toggle('selected');
            
            // Update booking summary
            updateBookingSummary();
        });
    });
}

function updateBookingSummary() {
    const selectedServices = document.querySelectorAll('.service-card.selected');
    const summaryContainer = document.getElementById('bookingSummary');
    
    if (!summaryContainer) return;
    
    let total = 0;
    let servicesHtml = '';
    
    selectedServices.forEach(card => {
        const name = card.querySelector('.card-title')?.textContent || card.querySelector('h5')?.textContent;
        const price = card.querySelector('.service-price')?.textContent || card.querySelector('.fw-bold')?.textContent;
        const priceValue = parseFloat(price?.replace('$', '') || 0);
        
        total += priceValue;
        servicesHtml += `
            <div class="summary-item d-flex justify-content-between">
                <span>${name}</span>
                <span>${price}</span>
            </div>
        `;
    });
    
    const summaryHtml = `
        ${servicesHtml}
        <div class="summary-item d-flex justify-content-between pt-3 mt-3 border-top">
            <span class="fw-bold">Total</span>
            <span class="total-price">$${total.toFixed(2)}</span>
        </div>
    `;
    
    summaryContainer.innerHTML = summaryHtml || '<p class="text-muted">No services selected</p>';
}

// ===== BOOKING FORM =====
function initBookingForm() {
    const bookingForm = document.getElementById('bookingForm');
    if (!bookingForm) return;
    
    bookingForm.addEventListener('submit', function(e) {
        e.preventDefault();
        
        // Collect form data
        const formData = new FormData(bookingForm);
        const data = Object.fromEntries(formData.entries());
        
        // Get selected services
        const selectedServices = [];
        document.querySelectorAll('.service-card.selected').forEach(card => {
            selectedServices.push(card.querySelector('h5')?.textContent);
        });
        
        // Get selected time
        const selectedTime = document.querySelector('.time-slot.selected')?.dataset.time;
        
        // Show confirmation
        if (selectedServices.length > 0 && selectedTime) {
            showNotification('Appointment booked successfully!', 'success');
            
            // Reset form after delay
            setTimeout(() => {
                window.location.href = 'customer-dashboard.html';
            }, 2000);
        } else {
            showNotification('Please select at least one service and time slot', 'error');
        }
    });
    
    // Listen for date/time selection
    document.addEventListener('dateSelected', (e) => {
        const dateInput = document.getElementById('selectedDate');
        if (dateInput) dateInput.value = e.detail.date;
    });
    
    document.addEventListener('timeSelected', (e) => {
        const timeInput = document.getElementById('selectedTime');
        if (timeInput) timeInput.value = e.detail.time;
    });
}

// ===== STAFF DASHBOARD =====
function initStaffDashboard() {
    const updateStatusBtns = document.querySelectorAll('.update-status-btn');
    
    updateStatusBtns.forEach(btn => {
        btn.addEventListener('click', function() {
            const appointmentId = this.dataset.id;
            const newStatus = this.dataset.status;
            
            // Update status badge
            const badge = document.querySelector(`.status-badge[data-id="${appointmentId}"]`);
            if (badge) {
                badge.className = `status-badge status-${newStatus}`;
                badge.textContent = newStatus.charAt(0).toUpperCase() + newStatus.slice(1);
            }
            
            // Show notification
            const statusText = newStatus === 'completed' ? 'completed' : 'marked as no-show';
            showNotification(`Appointment ${statusText}`, 'success');
            
            // Hide the action buttons
            const actions = this.closest('.appointment-actions');
            if (actions) actions.style.display = 'none';
        });
    });
    
    // Block time form
    const blockTimeForm = document.getElementById('blockTimeForm');
    if (blockTimeForm) {
        blockTimeForm.addEventListener('submit', function(e) {
            e.preventDefault();
            showNotification('Time blocked successfully', 'success');
            $('#blockTimeModal').modal('hide');
        });
    }
}

// ===== ADMIN DASHBOARD =====
function initAdminDashboard() {
    // Service management
    initServiceManagement();
    
    // Staff management
    initStaffManagement();
    
    // Reports - filter by date
    const dateFilter = document.getElementById('reportDateFilter');
    if (dateFilter) {
        dateFilter.addEventListener('change', function() {
            // In a real app, this would filter the data
            showNotification('Report filtered', 'success');
        });
    }
}

function initServiceManagement() {
    const addServiceBtn = document.getElementById('addServiceBtn');
    if (addServiceBtn) {
        addServiceBtn.addEventListener('click', () => {
            // Show add service modal
            $('#serviceModal').modal('show');
        });
    }
    
    const serviceForm = document.getElementById('serviceForm');
    if (serviceForm) {
        serviceForm.addEventListener('submit', function(e) {
            e.preventDefault();
            showNotification('Service added successfully', 'success');
            $('#serviceModal').modal('hide');
        });
    }
    
    // Edit/Delete service buttons
    document.querySelectorAll('.edit-service-btn').forEach(btn => {
        btn.addEventListener('click', function() {
            const serviceId = this.dataset.id;
            // In a real app, would populate form with service data
            $('#serviceModal').modal('show');
        });
    });
    
    document.querySelectorAll('.delete-service-btn').forEach(btn => {
        btn.addEventListener('click', function() {
            if (confirm('Are you sure you want to delete this service?')) {
                showNotification('Service deleted', 'success');
            }
        });
    });
}

function initStaffManagement() {
    const addStaffBtn = document.getElementById('addStaffBtn');
    if (addStaffBtn) {
        addStaffBtn.addEventListener('click', () => {
            $('#staffModal').modal('show');
        });
    }
    
    const staffForm = document.getElementById('staffForm');
    if (staffForm) {
        staffForm.addEventListener('submit', function(e) {
            e.preventDefault();
            showNotification('Staff member added successfully', 'success');
            $('#staffModal').modal('hide');
        });
    }
}

// ===== ANIMATIONS =====
function initAnimations() {
    // Fade in elements on scroll
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };
    
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('fade-in');
            }
        });
    }, observerOptions);
    
    document.querySelectorAll('.service-card, .staff-card, .stat-card').forEach(el => {
        observer.observe(el);
    });
}

// ===== NOTIFICATIONS =====
function showNotification(message, type = 'success') {
    // Remove existing notifications
    document.querySelectorAll('.notification').forEach(n => n.remove());
    
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.innerHTML = `
        <div class="d-flex align-items-center">
            <i class="fas fa-${type === 'success' ? 'check-circle' : 'exclamation-circle'} me-2"></i>
            <span>${message}</span>
        </div>
    `;
    
    document.body.appendChild(notification);
    
    // Auto remove after 3 seconds
    setTimeout(() => {
        notification.remove();
    }, 3000);
}

// ===== BOOKING CANCELLATION =====
function cancelBooking(bookingId) {
    if (confirm('Are you sure you want to cancel this booking?')) {
        // In a real app, would make API call
        const row = document.querySelector(`tr[data-booking-id="${bookingId}"]`);
        if (row) {
            const badge = row.querySelector('.status-badge');
            if (badge) {
                badge.className = 'status-badge status-cancelled';
                badge.textContent = 'Cancelled';
            }
        }
        showNotification('Booking cancelled', 'success');
    }
}

// ===== BOOKING RESCHEDULE =====
function rescheduleBooking(bookingId) {
    // In a real app, would open a modal with calendar
    $('#rescheduleModal').modal('show');
    
    // Store booking ID for reschedule
    window.currentRescheduleId = bookingId;
}

// ===== EXPORT FUNCTIONS FOR GLOBAL ACCESS =====
window.cancelBooking = cancelBooking;
window.rescheduleBooking = rescheduleBooking;
