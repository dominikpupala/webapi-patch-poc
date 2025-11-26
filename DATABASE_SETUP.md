# Database Setup

## Start the database

```bash
docker compose up -d
```

## Connection details

- **Server**: `localhost,1433` (from WSL2) or `<WSL2_IP>,1433` (from Windows)
- **Database**: `ProductsDb`
- **Username**: `sa`
- **Password**: `Devtest123!`

To get WSL2 IP (for Windows/SSMS):
```bash
ip addr show eth0 | grep "inet " | awk '{print $2}' | cut -d/ -f1
```

## Stop and remove

```bash
# Stop container (keeps data)
docker compose stop

# Remove container (keeps data in volume)
docker compose down

# Remove everything including data
docker compose down -v
```
