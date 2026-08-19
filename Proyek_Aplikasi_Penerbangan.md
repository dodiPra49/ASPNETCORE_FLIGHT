# Proyek ASP.NET Core: Aplikasi Jadwal Penerbangan (AviationStack API)

## 1. Pendahuluan
Aplikasi ini adalah aplikasi web berbasis ASP.NET Core MVC (atau Razor Pages) sederhana yang menampilkan jadwal penerbangan di seluruh dunia menggunakan sumber data dari API **AviationStack.com**. Aplikasi ini dirancang dengan satu form utama yang memiliki antarmuka pengguna (UI) yang modern, menarik, dan responsif untuk melakukan pencarian jadwal penerbangan.

## 2. Struktur Proyek (Optimasi untuk Visual Studio 2026)
Visual Studio 2026 bersama dengan rilis .NET terbaru (.NET 9 / .NET 10) mendukung struktur proyek yang lebih bersih dan efisien. Berikut adalah rekomendasi struktur proyek untuk skalabilitas dan kemudahan *maintenance*:

```text
📁 FlightScheduleApp
├── 📁 Controllers          # Menangani routing dan meneruskan data dari API ke View
│   └── HomeController.cs
├── 📁 Models               # Model data representasi JSON response dari AviationStack
│   ├── FlightResponse.cs
│   └── Flight.cs
├── 📁 Services             # Lapisan layanan (Business Logic) untuk berinteraksi dengan API eksternal
│   ├── IAviationStackService.cs
│   └── AviationStackService.cs
├── 📁 Views                # Antarmuka Pengguna
│   ├── 📁 Home
│   │   └── Index.cshtml    # Form utama pencarian jadwal (Single Page Layout)
│   └── 📁 Shared
│       └── _Layout.cshtml  # Layout utama dengan integrasi CSS/JS (Bootstrap/Tailwind)
├── 📁 wwwroot              # File statis (CSS, JS, Images)
│   ├── 📁 css
│   │   └── site.css        # Custom styling untuk mempercantik UI
│   └── 📁 js
│       └── site.js         # Logika AJAX untuk query data tanpa reload halaman
├── appsettings.json        # Konfigurasi aplikasi (Menyimpan API Key AviationStack dengan aman)
└── Program.cs              # Entry point aplikasi (Minimal hosting model, Dependency Injection)
```

## 3. Desain Antarmuka Pengguna (UI)
Untuk UI yang menarik dan modern, disarankan menggunakan **Bootstrap 5** atau **Tailwind CSS**. Kita menggunakan pendekatan *Single Form UI* di mana pengguna tidak perlu berpindah halaman saat melakukan pencarian (menggunakan AJAX/Fetch API).

### Komponen Utama di Halaman Pencarian (`Index.cshtml`):
1. **Hero Section (Header)**: Gambar latar belakang bertema bandara atau pesawat terbang dengan teks *greeting* dan judul yang jelas.
2. **Form Pencarian Interaktif**:
   - Input **Kode IATA Keberangkatan** (Departure) - *Contoh: CGK (Jakarta)*
   - Input **Kode IATA Kedatangan** (Arrival) - *Contoh: NRT (Tokyo)*
   - Tombol **Cari Penerbangan** dengan animasi hover dan indikator loading (*spinner*) saat proses *fetching* API berlangsung.
3. **Hasil Pencarian (Result UI)**:
   - Menampilkan hasil berupa daftar penerbangan yang elegan dalam bentuk **Card** atau **Tabel Responsif**.
   - Data yang ditampilkan: Logo/Nama Maskapai, Nomor Penerbangan, Waktu Keberangkatan, Waktu Kedatangan, Status (Scheduled, Active, Landed), dan Terminal/Gate.

## 4. Langkah-Langkah Implementasi Kode

### Langkah 1: Konfigurasi API Key
Daftar di [AviationStack](https://aviationstack.com/) dan dapatkan API Key Anda. Simpan di `appsettings.json`:
```json
{
  "AviationStack": {
    "ApiKey": "API_KEY_ANDA_DI_SINI",
    "BaseUrl": "http://api.aviationstack.com/v1/"
  },
  "AllowedHosts": "*"
}
```

### Langkah 2: Registrasi Service & HttpClient di `Program.cs`
Manfaatkan fitur `AddHttpClient` dan Dependency Injection di Visual Studio 2026:
```csharp
var builder = WebApplication.CreateBuilder(args);

// Tambahkan layanan MVC
builder.Services.AddControllersWithViews();

// Konfigurasi HTTP Client untuk AviationStack Service
builder.Services.AddHttpClient<IAviationStackService, AviationStackService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AviationStack:BaseUrl"]);
});

// Menambahkan In-Memory Caching untuk optimasi API call
builder.Services.AddMemoryCache(); 

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

### Langkah 3: Implementasi UI Form Pencarian (Razor Markup)
Berikut adalah contoh struktur UI untuk `Index.cshtml` menggunakan Bootstrap:
```html
<div class="container mt-5">
    <!-- Card Pencarian -->
    <div class="card shadow-lg border-0 rounded-lg">
        <div class="card-header bg-primary text-white text-center py-4 rounded-top">
            <h3 class="mb-0"><i class="fas fa-plane-departure"></i> Cari Jadwal Penerbangan Global</h3>
        </div>
        <div class="card-body p-4 bg-light">
            <form id="flightSearchForm">
                <div class="row g-3">
                    <div class="col-md-5">
                        <label for="depIata" class="form-label fw-bold">Keberangkatan (IATA)</label>
                        <input type="text" class="form-control form-control-lg text-uppercase" id="depIata" placeholder="Contoh: CGK" maxlength="3" required>
                    </div>
                    <div class="col-md-5">
                        <label for="arrIata" class="form-label fw-bold">Kedatangan (IATA)</label>
                        <input type="text" class="form-control form-control-lg text-uppercase" id="arrIata" placeholder="Contoh: NRT" maxlength="3">
                    </div>
                    <div class="col-md-2 d-flex align-items-end">
                        <button type="button" class="btn btn-primary btn-lg w-100 shadow" id="btnSearch">
                            Cari <i class="fas fa-search ms-1"></i>
                        </button>
                    </div>
                </div>
            </form>
        </div>
    </div>
    
    <!-- Container untuk menampilkan hasil atau indikator loading -->
    <div id="loadingIndicator" class="text-center mt-5 d-none">
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Loading...</span>
        </div>
        <p class="mt-2 text-muted">Mengambil data dari AviationStack...</p>
    </div>

    <div id="flightResults" class="mt-4"></div>
</div>
```

## 5. Fitur Optimasi Khusus Visual Studio 2026 & .NET
Untuk memaksimalkan *developer experience* di Visual Studio 2026:
1. **Gunakan Caching (`IMemoryCache`)**: API tingkat gratis dari AviationStack memiliki batas *request* bulanan yang sangat terbatas (contoh: 100 requests). Pastikan untuk menyimpan data pencarian serupa di memori selama 5-10 menit.
2. **C# 12/13 Features**: Gunakan fitur *Primary Constructors* pada kelas Service Anda untuk injeksi *HttpClient* dan konfigurasi.
3. **Hot Reload**: Gunakan fitur Hot Reload di VS 2026 agar setiap perubahan pada antarmuka pengguna (`.cshtml` atau CSS) langsung diterapkan ke peramban tanpa harus *rebuild* proyek.
4. **Asynchronous End-to-End**: Selalu pastikan metode pemanggilan HTTP menggunakan `async/await` agar *thread-pool* tidak terblokir selama menunggu *response* dari server AviationStack.
