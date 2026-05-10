import type {
  CreateProductRequest,
  PagedResponse,
  Product,
  UpdateProductRequest,
} from '../types/product';
import apiClient from '../utils/apiClient';

export const productService = {
  async getAll(page: number, pageSize: number): Promise<PagedResponse<Product>> {
    const { data } = await apiClient.get<PagedResponse<Product>>('/products', {
      params: { page, pageSize },
    });
    return data;
  },

  async getById(id: number): Promise<Product> {
    const { data } = await apiClient.get<Product>(`/products/${id}`);
    return data;
  },

  async create(request: CreateProductRequest): Promise<Product> {
    const { data } = await apiClient.post<Product>('/products', request);
    return data;
  },

  async update(id: number, request: UpdateProductRequest): Promise<Product> {
    const { data } = await apiClient.put<Product>(`/products/${id}`, request);
    return data;
  },

  async remove(id: number): Promise<void> {
    await apiClient.delete(`/products/${id}`);
  },

  async getCategories(): Promise<string[]> {
    const { data } = await apiClient.get<string[]>('/products/categories');
    return data;
  },
};
