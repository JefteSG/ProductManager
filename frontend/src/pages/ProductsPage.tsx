import axios from 'axios';
import { useState } from 'react';
import { ErrorMessage } from '../components/ErrorMessage';
import { Pagination } from '../components/Pagination';
import { ProductForm } from '../components/ProductForm';
import { ProductTable } from '../components/ProductTable';
import { useProducts } from '../hooks/useProducts';
import { productService } from '../services/productService';
import type { Product } from '../types/product';

export function ProductsPage() {
  const { data, loading, error, page, setPage, refresh } = useProducts(10);
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [deleteError, setDeleteError] = useState<string | null>(null);

  const handleAdd = () => {
    setEditingProduct(null);
    setShowForm(true);
  };

  const handleEdit = (product: Product) => {
    setEditingProduct(product);
    setShowForm(true);
  };

  const handleFormSuccess = () => {
    setShowForm(false);
    setEditingProduct(null);
    refresh();
  };

  const handleDelete = async (product: Product) => {
    if (!window.confirm(`Delete product "${product.name}"?`)) return;
    setDeleteError(null);
    try {
      await productService.remove(product.id);
      refresh();
    } catch (err) {
      if (axios.isAxiosError(err) && err.response?.data?.message) {
        setDeleteError(err.response.data.message as string);
      } else {
        setDeleteError('Failed to delete product.');
      }
    }
  };

  return (
    <div style={{ maxWidth: '1100px', margin: '0 auto', padding: '32px 16px' }}>
      {/* Header */}
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '24px' }}>
        <h1 style={{ margin: 0, fontSize: '24px', fontWeight: 700, color: '#111827' }}>
          Product Manager
        </h1>
        <button onClick={handleAdd} style={addBtnStyle}>
          + Add Product
        </button>
      </div>

      <ErrorMessage message={error} />
      <ErrorMessage message={deleteError} />

      {loading && <p style={{ color: '#6b7280' }}>Loading…</p>}

      {!loading && data && (
        <>
          <p style={{ fontSize: '13px', color: '#6b7280', marginBottom: '8px' }}>
            {data.totalItems} product{data.totalItems !== 1 ? 's' : ''} found
          </p>
          <ProductTable
            products={data.items}
            onEdit={handleEdit}
            onDelete={handleDelete}
          />
          <Pagination
            page={page}
            totalPages={data.totalPages}
            onPageChange={setPage}
          />
        </>
      )}

      {showForm && (
        <ProductForm
          product={editingProduct}
          onSuccess={handleFormSuccess}
          onCancel={() => { setShowForm(false); setEditingProduct(null); }}
        />
      )}
    </div>
  );
}

const addBtnStyle: React.CSSProperties = {
  background: '#16a34a',
  color: '#fff',
  border: 'none',
  borderRadius: '6px',
  padding: '9px 18px',
  cursor: 'pointer',
  fontSize: '14px',
  fontWeight: 600,
};
