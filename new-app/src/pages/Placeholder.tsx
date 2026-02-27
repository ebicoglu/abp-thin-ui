import { useLocation } from "react-router-dom";

const Placeholder = () => {
  const location = useLocation();
  const title = location.pathname.slice(1).replace(/^\w/, (c) => c.toUpperCase()) || "Page";

  return (
    <div>
      <h1 className="text-2xl font-semibold tracking-tight text-foreground">{title}</h1>
      <p className="mt-1 text-sm text-muted-foreground">This module is not yet implemented.</p>
    </div>
  );
};

export default Placeholder;
