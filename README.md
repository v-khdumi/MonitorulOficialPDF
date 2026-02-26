# Monitorul Oficial PDF

Aplicație web pentru extragerea și vizualizarea publicațiilor din [Monitorul Oficial al României](https://monitoruloficial.ro/) din ultimele 7 zile, cu suport pentru OCR și analiză AI.

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fv-khdumi%2FMonitorulOficialPDF%2Fmaster%2Fazuredeploy.json)

## Funcționalități

- **Extragere automată** — Scraping automat al publicațiilor din ultimele 7 zile de pe monitoruloficial.ro
- **Vizualizare PDF** — Afișarea documentelor PDF originale direct în browser
- **OCR (Recunoaștere optică a caracterelor)** — Extragere text din PDF-uri folosind Azure Document Intelligence
- **Analiză AI** — Rezumat și analiză inteligentă a textului extras, folosind Azure OpenAI (GPT)
- **Interfață modernă** — Design responsive cu tematică inspirată de tricolorul românesc

## Arhitectură

```
MonitorulOficialPDF.Web/          ← Aplicația ASP.NET Core 8.0
├── Models/
│   └── PublicationInfo.cs        ← Model pentru publicații
├── Services/
│   ├── MonitorScraperService.cs  ← Scraping monitoruloficial.ro
│   ├── PdfDownloadService.cs     ← Descărcare și cache PDF-uri
│   ├── OcrService.cs             ← Azure Document Intelligence OCR
│   └── AzureOpenAiService.cs     ← Azure OpenAI analiză text
├── Pages/
│   ├── Index.cshtml              ← Lista publicațiilor (ultimele 7 zile)
│   ├── ViewPdf.cshtml            ← Vizualizare PDF original
│   ├── OcrView.cshtml            ← Text OCR + Analiză AI
│   └── Api/Pdf.cshtml            ← API endpoint pentru servirea PDF-urilor
├── Program.cs                    ← Configurare DI și HTTP clients
└── appsettings.json              ← Configurare aplicație

azuredeploy.json                  ← ARM template pentru Deploy to Azure
azuredeploy.parameters.json       ← Parametri ARM template
```

## Servicii Azure necesare

| Serviciu | Descriere | Obligatoriu |
|----------|-----------|-------------|
| **Azure App Service** | Găzduire aplicație web (Windows, F1 free tier implicit) | Da |
| **Azure Document Intelligence** | OCR pentru extragere text din PDF-uri | Da (provizionat automat de template) |
| **Azure OpenAI** | Analiză și rezumat inteligent al textului | Da (provizionat automat de template) |

> Ambele servicii Azure (Document Intelligence și OpenAI) sunt provizionate automat prin template-ul ARM. Scraping-ul și vizualizarea PDF-urilor funcționează independent de aceste servicii.

## Deployment în Azure

### Opțiunea 1: Deploy to Azure (recomandat)

Apasă butonul de mai sus sau acest link:

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fv-khdumi%2FMonitorulOficialPDF%2Fmaster%2Fazuredeploy.json)

Completează parametrii:
1. **Web App Name** — Numele aplicației (unic în Azure)
2. **Location** — Regiunea Azure (recomandat: `West Europe`)
3. **SKU** — Pricing tier (default: `F1` gratuit; opțiune: `B1` basic plătit cu VM dedicat). D1 (shared) nu este inclus deoarece majoritatea subscripțiilor au cotă zero pentru Shared VMs.
4. **Document Intelligence SKU** — Tier pentru OCR (default: `F0` gratuit, sau `S0` standard)
5. **OpenAI Deployment Name** — Numele deployment-ului Azure OpenAI (default: `gpt-4o`)
6. **OpenAI Model Name/Version** — Modelul și versiunea Azure OpenAI (default: `gpt-4o` / `2024-08-06`)

> **Notă:** Template-ul provizionează automat un cont Azure OpenAI, un deployment de model și un cont Azure Document Intelligence. Nu este necesar să creați manual aceste resurse.

### Opțiunea 2: Deployment manual

```bash
# Clonează repository-ul
git clone https://github.com/v-khdumi/MonitorulOficialPDF.git
cd MonitorulOficialPDF

# Build și rulare locală
cd MonitorulOficialPDF.Web
dotnet run

# Sau publish și deploy
dotnet publish -c Release -o ./publish
```

### Opțiunea 3: Azure CLI

```bash
# Creează resursele Azure
az group create --name MonitorulOficial-RG --location westeurope

az deployment group create \
  --resource-group MonitorulOficial-RG \
  --template-file azuredeploy.json \
  --parameters azuredeploy.parameters.json
```

## Configurare

### Variabile de mediu / App Settings

| Variabilă | Descriere |
|-----------|-----------|
| `AzureDocumentIntelligence__Endpoint` | URL endpoint Azure Document Intelligence |
| `AzureDocumentIntelligence__Key` | Cheie API Azure Document Intelligence |
| `AzureOpenAI__Endpoint` | URL endpoint Azure OpenAI |
| `AzureOpenAI__Key` | Cheie API Azure OpenAI |
| `AzureOpenAI__DeploymentName` | Numele deployment-ului Azure OpenAI (default: `gpt-4o`) |

### appsettings.json

```json
{
  "AzureDocumentIntelligence": {
    "Endpoint": "https://<your-resource>.cognitiveservices.azure.com/",
    "Key": "<your-key>"
  },
  "AzureOpenAI": {
    "Endpoint": "https://<your-resource>.openai.azure.com/",
    "Key": "<your-key>",
    "DeploymentName": "gpt-4o"
  }
}
```

## Dezvoltare locală

### Cerințe
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Rulare
```bash
cd MonitorulOficialPDF.Web
dotnet run
```

Aplicația va fi disponibilă la `http://localhost:5283`.

## Tehnologii utilizate

- **ASP.NET Core 8.0** — Framework web
- **Razor Pages** — Interfață utilizator
- **Bootstrap 5** — Design responsive
- **HtmlAgilityPack** — Parsing HTML pentru scraping
- **Azure Document Intelligence** — OCR (Form Recognizer)
- **Azure OpenAI** — Analiză text cu GPT
- **Azure App Service (Windows)** — Hosting
