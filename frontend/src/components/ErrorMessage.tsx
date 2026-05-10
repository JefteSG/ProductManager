interface ErrorMessageProps {
  message: string | null;
}

export function ErrorMessage({ message }: ErrorMessageProps) {
  if (!message) return null;

  return (
    <div
      role="alert"
      style={{
        background: '#fef2f2',
        border: '1px solid #fca5a5',
        borderRadius: '6px',
        color: '#b91c1c',
        padding: '10px 14px',
        marginBottom: '16px',
        fontSize: '14px',
      }}
    >
      {message}
    </div>
  );
}
