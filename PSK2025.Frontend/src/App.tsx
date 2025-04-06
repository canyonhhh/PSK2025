import RandomNumber from "./components/RandomNumber"; // Import the RandomNumber component
import {
  QueryClient,
  QueryClientProvider,
} from '@tanstack/react-query'

const queryClient = new QueryClient();

function App() {
  return (
        <QueryClientProvider client={queryClient}>
          <div>
            <h1>Random Number Generator</h1>
            <RandomNumber /> {/* Render the component */}
          </div>
        </QueryClientProvider>
  );
}

export default App;
