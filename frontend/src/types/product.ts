export interface Product {
  id: number;
  name: string;
  sku: string;
  category: string;
  description?: string;
  price: number;
  stockQuantity: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateProductRequest {
  name: string;
  sku: string;
  category: string;
  description?: string;
  price: number;
  stockQuantity: number;
}

export interface UpdateProductRequest {
  name: string;
  sku: string;
  category: string;
  description?: string;
  price: number;
  stockQuantity: number;
}

export interface PagedResponse<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}

export interface ErrorResponse {
  message: string;
  statusCode: number;
}
