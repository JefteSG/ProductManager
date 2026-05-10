import type { Product } from '../types/product';

interface ProductTableProps {
  products: Product[];
  onEdit: (product: Product) => void;
  onDelete: (product: Product) => void;
}

export function ProductTable({ products, onEdit, onDelete }: ProductTableProps) {
  if (products.length === 0) {
    return <p style={{ color: '#6b7280', textAlign: 'center', padding: '24px 0' }}>No products found.</p>;
  }

  return (
    <div style={{ overflowX: 'auto' }}>
      <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: '14px' }}>
        <thead>
          <tr style={{ background: '#f3f4f6', textAlign: 'left' }}>
            <Th>ID</Th>
            <Th>Name</Th>
            <Th>SKU</Th>
            <Th>Category</Th>
            <Th>Price (R$)</Th>
            <Th>Stock</Th>
            <Th>Actions</Th>
          </tr>
        </thead>
        <tbody>
          {products.map((p) => (
            <tr key={p.id} style={{ borderBottom: '1px solid #e5e7eb' }}>
              <Td>{p.id}</Td>
              <Td>{p.name}</Td>
              <Td><code style={{ background: '#f3f4f6', padding: '2px 6px', borderRadius: '4px', color: '#111827' }}>{p.sku}</code></Td>
              <Td>{p.category}</Td>
              <Td>{p.price.toFixed(2)}</Td>
              <Td>
                <span style={{ color: p.stockQuantity === 0 ? '#ef4444' : 'inherit' }}>
                  {p.stockQuantity}
                </span>
              </Td>
              <Td>
                <button
                  onClick={() => onEdit(p)}
                  style={btnStyle('#2563eb')}
                >
                  Edit
                </button>
                <button
                  onClick={() => onDelete(p)}
                  style={{ ...btnStyle('#dc2626'), marginLeft: '8px' }}
                >
                  Delete
                </button>
              </Td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

function Th({ children }: { children: React.ReactNode }) {
  return (
    <th style={{ padding: '10px 12px', fontWeight: 600, color: '#374151' }}>{children}</th>
  );
}

function Td({ children }: { children: React.ReactNode }) {
  return (
    <td style={{ padding: '10px 12px', color: '#111827' }}>{children}</td>
  );
}

function btnStyle(color: string): React.CSSProperties {
  return {
    background: color,
    color: '#fff',
    border: 'none',
    borderRadius: '4px',
    padding: '5px 12px',
    cursor: 'pointer',
    fontSize: '13px',
  };
}
