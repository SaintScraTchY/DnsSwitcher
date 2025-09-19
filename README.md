# QuickDns

**QuickDns** is a lightweight, fast, and straightforward console application for managing your DNS settings on Windows.  
Designed for speed, simplicity, and minimal overhead.

---

## Features

- List and select configured DNS servers quickly
- Apply DNS settings instantly
- View current and historical DNS usage
- Lightweight console interface with optional enhanced table dashboard
- Easy keyboard navigation (Arrow keys, Enter, ESC to cancel)
- Fully configurable settings (colors, dashboard style, history limit)
- Portable single-file executable with embedded icon

---

## Getting Started

### Build from source

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/QuickDns.git
   cd QuickDns```

2. Build using .NET CLI:

   ```bash
   dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
   ```

3. Run the executable from `bin/Release/net9.0/win-x64/publish/QuickDns.exe`

> Windows only for now. Linux/macOS support might be added later.

---

## Configuration

* `settings.json` allows customization of:

  * Accent color
  * Highlight colors
  * Dashboard style (simple or table)
  * History limit

> If `settings.json` does not exist, QuickDns will create a default one automatically.

---
