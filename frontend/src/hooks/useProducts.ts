import { useCallback, useEffect, useState } from 'react';
import { productService } from '../services/productService';
import { getApiErrorMessage } from '../utils/apiClient';
import type { PagedResponse, Product } from '../types/product';

interface UseProductsReturn {
  data: PagedResponse<Product> | null;
  loading: boolean;
  error: string | null;
  page: number;
  pageSize: number;
  setPage: (page: number) => void;
  refresh: () => void;
}

/**
 * Encapsulates all product list state: fetching, pagination and error handling.
 * Components only call refresh() after mutations — they don't touch apiClient directly.
 */
export function useProducts(initialPageSize = 10): UseProductsReturn {
  const [data, setData] = useState<PagedResponse<Product> | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(initialPageSize);
  const [tick, setTick] = useState(0);

  const refresh = useCallback(() => setTick((t) => t + 1), []);

  useEffect(() => {
    let cancelled = false;

    const load = async () => {
      setLoading(true);
      setError(null);
      try {
        const result = await productService.getAll(page, pageSize);
        if (!cancelled) setData(result);
      } catch (err) {
        if (!cancelled) {
          setError(getApiErrorMessage(err, 'Failed to load products.'));
        }
      } finally {
        if (!cancelled) setLoading(false);
      }
    };

    load();
    return () => { cancelled = true; };
  }, [page, pageSize, tick]);

  return { data, loading, error, page, pageSize, setPage, refresh };
}
