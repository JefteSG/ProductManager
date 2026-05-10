import { useEffect, useState } from 'react';
import { productService } from '../services/productService';
import { getApiErrorMessage } from '../utils/apiClient';
import type { CreateProductRequest, Product, UpdateProductRequest } from '../types/product';
import { ErrorMessage } from './ErrorMessage';

interface ProductFormProps {
  product?: Product | null;
  onSuccess: () => void;
  onCancel: () => void;
}

const EMPTY_FORM: CreateProductRequest = {
  name: '',
  sku: '',
  category: '',
  description: '',
  price: 0,
  stockQuantity: 0,
};

/**
 * Controlled form used for both create and edit.
 * The same component is reused by passing in a product prop (edit mode)
 * or leaving it undefined (create mode).
 */
export function ProductForm({ product, onSuccess, onCancel }: ProductFormProps) {
  const [form, setForm] = useState<CreateProductRequest>(EMPTY_FORM);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [categories, setCategories] = useState<string[]>([]);

  const isEditing = !!product;

  // Fetch categories from the API so the list stays in sync with the backend
  useEffect(() => {
    productService.getCategories().then(setCategories).catch(() => {
      // Fall back to empty — the select will just show no options
    });
  }, []);

  // Populate form when editing
  useEffect(() => {
    if (product) {
      setForm({
        name: product.name,
        sku: product.sku,
        category: product.category,
        description: product.description ?? '',
        price: product.price,
        stockQuantity: product.stockQuantity,
      });
    } else {
      setForm(EMPTY_FORM);
    }
    setError(null);
  }, [product]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setForm((prev) => ({
      ...prev,
      [name]: name === 'price' || name === 'stockQuantity' ? Number(value) : value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSubmitting(true);

    try {
      if (isEditing) {
        const update: UpdateProductRequest = { ...form };
        await productService.update(product!.id, update);
      } else {
        await productService.create(form);
      }
      onSuccess();
    } catch (err) {
      setError(getApiErrorMessage(err, 'An unexpected error occurred. Please try again.'));
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div style={overlayStyle}>
      <div style={modalStyle}>
        <h2 style={{ margin: '0 0 20px', fontSize: '18px', color: '#111827' }}>
          {isEditing ? 'Edit Product' : 'Add Product'}
        </h2>

        <ErrorMessage message={error} />

        <form onSubmit={handleSubmit} noValidate>
          <Field label="Name *">
            <input name="name" value={form.name} onChange={handleChange} required style={inputStyle} />
          </Field>

          <Field label="SKU *">
            <input name="sku" value={form.sku} onChange={handleChange} required style={inputStyle} />
          </Field>

          <Field label="Category *">
            <select name="category" value={form.category} onChange={handleChange} required style={inputStyle}>
              <option value="">Select a category…</option>
              {categories.map((c) => (
                <option key={c} value={c}>{c}</option>
              ))}
            </select>
          </Field>

          <Field label={`Price (R$) *${form.category === 'Eletronicos' ? '  — min R$ 50.00 for Eletronicos' : ''}`}>
            <input
              name="price"
              type="number"
              min={0}
              step="0.01"
              value={form.price}
              onChange={handleChange}
              required
              style={inputStyle}
            />
          </Field>

          <Field label="Stock Quantity *">
            <input
              name="stockQuantity"
              type="number"
              min={0}
              value={form.stockQuantity}
              onChange={handleChange}
              required
              style={inputStyle}
            />
          </Field>

          <Field label="Description">
            <textarea
              name="description"
              value={form.description}
              onChange={handleChange}
              rows={3}
              style={{ ...inputStyle, resize: 'vertical' }}
            />
          </Field>

          <div style={{ display: 'flex', gap: '10px', justifyContent: 'flex-end', marginTop: '8px' }}>
            <button type="button" onClick={onCancel} style={cancelBtnStyle} disabled={submitting}>
              Cancel
            </button>
            <button type="submit" style={submitBtnStyle} disabled={submitting}>
              {submitting ? 'Saving…' : isEditing ? 'Update' : 'Create'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

function Field({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <div style={{ marginBottom: '14px' }}>
      <label style={{ display: 'block', fontSize: '13px', fontWeight: 500, color: '#374151', marginBottom: '4px' }}>
        {label}
      </label>
      {children}
    </div>
  );
}

const overlayStyle: React.CSSProperties = {
  position: 'fixed', inset: 0,
  background: 'rgba(0,0,0,0.4)',
  display: 'flex', alignItems: 'center', justifyContent: 'center',
  zIndex: 50,
};

const modalStyle: React.CSSProperties = {
  background: '#fff',
  borderRadius: '8px',
  padding: '28px 32px',
  width: '100%',
  maxWidth: '500px',
  maxHeight: '90vh',
  overflowY: 'auto',
  boxShadow: '0 20px 40px rgba(0,0,0,0.15)',
};

const inputStyle: React.CSSProperties = {
  width: '100%',
  padding: '7px 10px',
  border: '1px solid #d1d5db',
  borderRadius: '4px',
  fontSize: '14px',
  boxSizing: 'border-box',
};

const submitBtnStyle: React.CSSProperties = {
  background: '#2563eb',
  color: '#fff',
  border: 'none',
  borderRadius: '5px',
  padding: '8px 20px',
  cursor: 'pointer',
  fontSize: '14px',
};

const cancelBtnStyle: React.CSSProperties = {
  background: '#f3f4f6',
  color: '#374151',
  border: '1px solid #d1d5db',
  borderRadius: '5px',
  padding: '8px 20px',
  cursor: 'pointer',
  fontSize: '14px',
};
