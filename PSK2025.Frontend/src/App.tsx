import { BrowserRouter, Routes, Route } from 'react-router-dom';
import RandomNumber from './components/RandomNumber'; // Import the RandomNumber component
import {
  QueryClient,
  QueryClientProvider,
} from '@tanstack/react-query';

const queryClient = new QueryClient();

function App() {
  return (
        <QueryClientProvider client={queryClient}>
            <BrowserRouter>
                <Routes>
                    <Route path="/" element={<RandomNumber />} />
                </Routes>
            </BrowserRouter>
        </QueryClientProvider>
  );
}

export default App;
