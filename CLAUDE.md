# ResultWrapper — Loyiha bo'yicha qo'llanma

## Loyiha haqida

**ResultWrapper** — ASP.NET Core API loyihalarida barcha javoblarni yagona formatga o'rash (wrap qilish) uchun mo'ljallangan oddiy va engil .NET kutubxonasi. NuGet orqali tarqatiladi.

- **NuGet paketi:** `ResultWrapper.Library`
- **Versiya:** 2.0.0
- **Target framework:** .NET 8.0
- **Muallif:** @0605AbMu
- **Litsenziya:** Unlicense

## Loyiha tuzilmasi

```
result-wrapper/
├── src/
│   ├── ResultWrapper.Library/              # Asosiy kutubxona
│   │   ├── Common/
│   │   │   └── ModelError.cs               # Model xatoliklari uchun yordamchi klass
│   │   ├── Interfaces/
│   │   │   └── IWrapper.cs                 # IWrapper<T> interfeysi
│   │   ├── Wrapper.cs                      # Nogenerik Wrapper (Wrapper<object?> dan meros)
│   │   ├── Wrapper.Casts.cs                # Wrapper uchun implicit konvertatsiyalar
│   │   ├── WrapperGeneric.cs               # Generik Wrapper<T> asosiy klassi
│   │   ├── WrapperGeneric.Casts.cs         # Wrapper<T> uchun implicit konvertatsiyalar
│   │   └── ResultWrapper.Library.csproj
│   └── tests/
│       └── ResultWrapper.Test/             # NUnit test loyihasi
│           ├── GlobalUsings.cs
│           ├── ResultWrapper.Wrapper.Test.cs
│           └── ResultWrapper.WrapperGeneric.Test.cs
├── ResultWrapper.sln
├── README.md
└── .github/workflows/pack.yml              # CI/CD pipeline
```

## Asosiy arxitektura

### Sinf ierarxiyasi

```
IWrapper<T>  (interfeys)
    └── Wrapper<T>          (generik asosiy klass — partial, WrapperGeneric.cs + WrapperGeneric.Casts.cs)
            └── Wrapper     (nogenerik versiya — partial, Wrapper.cs + Wrapper.Casts.cs)
                            Wrapper<object?> dan meros oladi
```

### `IWrapper<T>` interfeysi (`Interfaces/IWrapper.cs`)

| Xususiyat | Turi | Tavsif |
|-----------|------|--------|
| `Id` | `string` | Avtomatik generatsiya qilinadigan so'rov identifikatori (`Activity.Current?.Id` yoki `Guid`) |
| `Code` | `int` | HTTP status kodi |
| `Content` | `T?` | Javob ma'lumoti |
| `Message` | `string?` | Xato yoki ma'lumot xabari |

### `Wrapper<T>` — Generik klass (`WrapperGeneric.cs`)

JSON serializatsiyasi uchun atributlar mavjud:
- `[JsonPropertyName("id")]` — `Id`
- `[JsonPropertyName("code")]` — `Code`
- `[JsonPropertyName("content")]` — `Content`
- `[JsonPropertyName("message")]` — `Message`

### `ModelError` klassi (`Common/ModelError.cs`)

Model validatsiya xatolarini saqlash uchun. `Key` (maydon nomi) va `ErrorMessage` xususiyatlariga ega. `[JsonConstructor]` atributi bilan JSON deserializatsiyasini qo'llab-quvvatlaydi.

## Factory metodlar

Barcha factory metodlar `static` bo'lib, `new()` orqali chaqiriladi.

### `FromSuccess` — Muvaffaqiyatli natija

```csharp
Wrapper.FromSuccess(content)                    // 200 OK
Wrapper.FromSuccess(content, 201)               // Raqamli kod bilan
Wrapper.FromSuccess(content, HttpStatusCode.OK) // HttpStatusCode bilan

Wrapper<string>.FromSuccess("natija")
Wrapper<string>.FromSuccess("natija", HttpStatusCode.Created)
```

### `FromError` — Xato natijasi

```csharp
Wrapper.FromError(exception)                         // 500 InternalServerError
Wrapper.FromError(exception, HttpStatusCode.BadRequest)
Wrapper.FromError(exception, 400)
Wrapper.FromError("Xato xabari")                    // Faqat matn bilan

Wrapper<T>.FromError(exception)
```

### `FromModelState` — Validatsiya xatoliklari

```csharp
Wrapper.FromModelState(ModelState)                   // 400, xabarsiz
Wrapper.FromModelState(ModelState, "Xato!")         // 400, xabar bilan
Wrapper.FromModelState(ModelState, exception)        // 400, exception.Message
Wrapper.FromModelState(ModelState, "Xato", 422)     // Maxsus kod bilan
```

Qaytaradi: `Wrapper<IReadOnlyDictionary<string, string?>>` — `{ "maydonNomi": "xato xabari" }` formatida.

### `FromStatus` — Faqat status kodi

```csharp
Wrapper.FromStatus(200)
Wrapper.FromStatus(HttpStatusCode.NotFound)
```

## Fluent metodlar (zanjirli chaqiruv)

```csharp
Wrapper.FromSuccess(data)
    .WithId("maxsus-id")
    .WithCode(HttpStatusCode.Created);
```

## Implicit konvertatsiyalar (avtomatik casting)

`Wrapper.Casts.cs` va `WrapperGeneric.Casts.cs` fayllarida aniqlangan.

### `Wrapper` uchun:

```csharp
Wrapper w1 = "matn";                        // FromSuccess("matn")
Wrapper w2 = 200;                           // FromStatus(200)
Wrapper w3 = HttpStatusCode.OK;             // FromStatus(HttpStatusCode.OK)
Wrapper w4 = exception;                     // FromError(exception)
Wrapper w5 = (ma'lumot, 201);              // FromSuccess(ma'lumot, 201)
Wrapper w6 = (exception, 400);             // FromError(exception, 400)
```

### `Wrapper<T>` uchun:

```csharp
Wrapper<string> w1 = "matn";               // FromSuccess("matn")
Wrapper<int> w2 = 42;                      // FromSuccess(42)
Wrapper<string> w3 = HttpStatusCode.OK;    // FromStatus
Wrapper<string> w4 = exception;            // FromError(exception)
Wrapper<string> w5 = ("natija", 200);     // FromSuccess("natija", 200)

// Generikdan nogenerikka
Wrapper nogenerik = wrapper_generik;       // Content object? sifatida saqlanadi
```

## Asosiy foydalanish namunalari

### API kontrollerida

```csharp
// Faqat status
[HttpGet]
public Wrapper OnlyStatus() => HttpStatusCode.OK;

// Muvaffaqiyatli natija
[HttpGet]
public Wrapper Add(int a, int b) => Wrapper.FromSuccess(a + b);

// Matn qaytarish (implicit cast)
[HttpGet]
public Wrapper TextResult() => "Bajarildi!";

// Tuple bilan
[HttpGet]
public Wrapper WithCode() => (new { Sum = 5 }, 200);

// Xato ushlash
[HttpGet]
public Wrapper Error()
{
    try { throw new InvalidOperationException(); }
    catch (Exception e) { return Wrapper.FromError(e, HttpStatusCode.BadRequest); }
}

// Model validatsiyasi
[HttpPost]
public Wrapper Create([FromBody] Model model)
{
    if (!ModelState.IsValid)
        return Wrapper.FromModelState(ModelState);
    return Wrapper.FromStatus(HttpStatusCode.OK);
}

// Strong-typed
[HttpGet]
public Wrapper<string> Typed() => "Natija";
```

### JSON javob formati

```json
{
    "id": "463fadea-066e-4950-a733-5bc789a9ea94",
    "code": 200,
    "content": { ... },
    "message": null
}
```

## Testlar

**Test framework:** NUnit 3.13.3

### Testlarni ishlatish

```bash
# Barcha testlarni ishlatish
dotnet test src/tests/ResultWrapper.Test/ResultWrapper.Test.csproj

# Bitta test faylini ishlatish
dotnet test --filter "FullyQualifiedName~ResultWrapperTest"
```

### Test fayllari

| Fayl | Sinov obyekti |
|------|---------------|
| `ResultWrapper.Wrapper.Test.cs` | `Wrapper` (nogenerik) uchun testlar |
| `ResultWrapper.WrapperGeneric.Test.cs` | `Wrapper<T>` (generik) uchun testlar |

### Qamrab olingan ssenarilar

- `Id` → `Activity.Current?.Id` yoki yangi `Guid` sifatida to'g'ri generatsiya
- `FromSuccess` → `Content` va `Code` to'g'ri o'rnatiladi
- `FromError` → `Message` to'g'ri, `Content` null
- `FromStatus` → faqat `Code` o'rnatiladi
- `FromModelState` → `IReadOnlyDictionary<string, string?>` formatida xatolar
- JSON deserializatsiyasi → `Wrapper` va `Wrapper<T>` to'g'ri deserializatsiya bo'ladi
- Implicit casting → `Wrapper<T>` → `Wrapper` konvertatsiyasi

## Build va CI/CD

### Lokal build

```bash
dotnet build
dotnet test
dotnet pack src/ResultWrapper.Library/ResultWrapper.Library.csproj -c Release -p:PackageVersion=2.0.0
```

### GitHub Actions (`pack.yml`)

Tag push qilinganda avtomatik ishga tushadi:

```
Tag push → Build → Test → Pack → Publish to NuGet
```

- **Build:** `dotnet build`
- **Test:** `dotnet test src/tests/ResultWrapper.Test/...`
- **Pack:** `dotnet pack -c Release -p:PackageVersion=${{ github.ref_name }}`
- **Publish:** NuGet.org ga `NUGET_API_KEY` secret orqali yuboriladi

### Yangi versiya chiqarish

```bash
git tag 2.1.0
git push origin 2.1.0
```

Tag nomi avtomatik ravishda NuGet paket versiyasi sifatida ishlatiladi.

## Muhim nozikliklar

- `Wrapper` klasi `Wrapper<object?>` dan meros oladi — nogenerik versiya generikni ixtisoslashtirgan holati.
- `Wrapper<T>` va `Wrapper` partial klass sifatida e'lon qilingan — asosiy mantiq va cast operatorlari alohida fayllarda.
- `Content` xususiyati `init` setter bilan aniqlangan — ob'ekt yaratilgandan keyin o'zgartirib bo'lmaydi.
- `Id` default qiymati `Activity.Current?.Id ?? Guid.NewGuid().ToString()` — distributed tracing bilan integratsiya qilingan.
- `FromModelState` doim `Wrapper<IReadOnlyDictionary<string, string?>>` qaytaradi, `Wrapper<T>` emas.
- `ModelError.cs` da `#pragma warning disable CS8618` mavjud — `Key` non-nullable field uchun parametrsiz konstruktor kerak bo'lgani uchun.
