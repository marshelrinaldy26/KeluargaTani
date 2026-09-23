window.App = {
  showAuthForm: function(type) {
    document.getElementById('form-login-box').classList.add('hidden');
    document.getElementById('form-register-box').classList.add('hidden');
    document.getElementById('form-forgot-box').classList.add('hidden');
    
    if (type === 'login') document.getElementById('form-login-box').classList.remove('hidden');
    if (type === 'register') document.getElementById('form-register-box').classList.remove('hidden');
    if (type === 'forgot') document.getElementById('form-forgot-box').classList.remove('hidden');
  },
  
  login: function(e) {
    if (e) e.preventDefault();
    document.getElementById('auth-container').classList.add('hidden');
    document.getElementById('app-shell').classList.remove('hidden');
    document.getElementById('app-shell').classList.add('flex', 'md:flex-row', 'flex-col');
    this.navigateTo('dashboard');
  },
  
  logout: function() {
    document.getElementById('app-shell').classList.add('hidden');
    document.getElementById('app-shell').classList.remove('flex', 'md:flex-row', 'flex-col');
    document.getElementById('auth-container').classList.remove('hidden');
  },
  
  navigateTo: function(viewId) {
    // Sembunyikan semua view
    document.querySelectorAll('.app-view').forEach(el => el.classList.add('hidden'));
    
    // Tampilkan view yang dituju
    const view = document.getElementById('view-' + viewId);
    if (view) view.classList.remove('hidden');
    
    // Update warna tombol navigasi di sidebar
    document.querySelectorAll('.nav-btn').forEach(btn => {
      btn.classList.remove('bg-emerald-800', 'text-white', 'bg-emerald-800/60');
      btn.classList.add('text-emerald-100');
    });
    const activeBtn = document.querySelector(`.nav-btn[data-view="${viewId}"]`);
    if (activeBtn) {
      activeBtn.classList.remove('text-emerald-100');
      activeBtn.classList.add('bg-emerald-800', 'text-white');
    }
    
    this.closeMobileSidebar();
  },
  
  toggleMobileSidebar: function() {
    const sidebar = document.getElementById('sidebar');
    const backdrop = document.getElementById('sidebar-backdrop');
    if (sidebar.classList.contains('-translate-x-full')) {
      sidebar.classList.remove('-translate-x-full');
      backdrop.classList.remove('hidden');
    } else {
      this.closeMobileSidebar();
    }
  },
  
  closeMobileSidebar: function() {
    const sidebar = document.getElementById('sidebar');
    const backdrop = document.getElementById('sidebar-backdrop');
    if (sidebar) sidebar.classList.add('-translate-x-full');
    if (backdrop) backdrop.classList.add('hidden');
  },
  
  toggleTheme: function() {
    document.documentElement.classList.toggle('dark');
  },
  
  loadDemoData: function() {
    alert("Ini adalah tampilan prototipe UI. Fitur Data Contoh belum diaktifkan.");
  },
  
  openSaleModal: function() {
    alert("Ini adalah tampilan prototipe UI. Form penjualan belum aktif.");
  },
  
  openImportModal: function() {
    alert("Ini adalah tampilan prototipe UI. Fitur Impor Excel belum diaktifkan.");
  },
  
  exportSalesExcel: function() {
    alert("Ini adalah tampilan prototipe UI. Fitur Ekspor belum diaktifkan.");
  },
  
  renderCharts: function() {
    console.log("Simulasi render grafik...");
  }
};

// Pasang event listener saat halaman dimuat
document.addEventListener('DOMContentLoaded', () => {
  const formLogin = document.getElementById('form-login');
  if (formLogin) {
    formLogin.addEventListener('submit', App.login.bind(App));
  }
  
  const formRegister = document.getElementById('form-register');
  if (formRegister) {
    formRegister.addEventListener('submit', App.login.bind(App));
  }
  
  // Deteksi tema dari OS
  if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
    document.documentElement.classList.add('dark');
  }
});

