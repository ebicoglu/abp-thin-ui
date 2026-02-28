import { useEffect, useState } from "react";

export interface Brand {
  appName: string;
  logo: string | null;
}

const defaultBrand: Brand = { appName: "My App", logo: null };

export function useBrand(): Brand {
  const [brand, setBrand] = useState<Brand>(defaultBrand);

  useEffect(() => {
    fetch("/brand.json")
      .then((r) => (r.ok ? r.json() : Promise.reject()))
      .then((data: { appName?: string; logo?: string | null }) => {
        setBrand({
          appName: data.appName ?? defaultBrand.appName,
          logo: data.logo ?? null,
        });
      })
      .catch(() => {});
  }, []);

  return brand;
}
