import { Navigate, Route, Routes } from "react-router-dom";
import "./App.css";
import { AppLayout } from "./components/AppLayout";
import { CreateTestPage } from "./pages/CreateTestPage";
import { EditTestPage } from "./pages/EditTestPage";
import { TakeTestPage } from "./pages/TakeTestPage";
import { TestDetailsPage } from "./pages/TestDetailsPage";
import { TestsListPage } from "./pages/TestsListPage";

function App() {
  return (
    <Routes>
      <Route element={<AppLayout />}>
        <Route index element={<TestsListPage />} />
        <Route path="tests/create" element={<CreateTestPage />} />
        <Route path="tests/:id" element={<TestDetailsPage />} />
        <Route path="tests/:id/edit" element={<EditTestPage />} />
        <Route path="tests/:id/take" element={<TakeTestPage />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Route>
    </Routes>
  );
}

export default App;
