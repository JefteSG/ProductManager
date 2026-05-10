interface PaginationProps {
  page: number;
  totalPages: number;
  onPageChange: (page: number) => void;
}

export function Pagination({ page, totalPages, onPageChange }: PaginationProps) {
  if (totalPages <= 1) return null;

  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginTop: '16px', justifyContent: 'center' }}>
      <PageBtn onClick={() => onPageChange(1)} disabled={page === 1}>«</PageBtn>
      <PageBtn onClick={() => onPageChange(page - 1)} disabled={page === 1}>‹</PageBtn>

      <span style={{ fontSize: '14px', color: '#374151' }}>
        Page {page} of {totalPages}
      </span>

      <PageBtn onClick={() => onPageChange(page + 1)} disabled={page === totalPages}>›</PageBtn>
      <PageBtn onClick={() => onPageChange(totalPages)} disabled={page === totalPages}>»</PageBtn>
    </div>
  );
}

function PageBtn({
  children,
  onClick,
  disabled,
}: {
  children: React.ReactNode;
  onClick: () => void;
  disabled: boolean;
}) {
  return (
    <button
      onClick={onClick}
      disabled={disabled}
      style={{
        padding: '4px 10px',
        border: '1px solid #d1d5db',
        borderRadius: '4px',
        background: disabled ? '#f3f4f6' : '#fff',
        color: disabled ? '#9ca3af' : '#374151',
        cursor: disabled ? 'not-allowed' : 'pointer',
        fontSize: '14px',
      }}
    >
      {children}
    </button>
  );
}
