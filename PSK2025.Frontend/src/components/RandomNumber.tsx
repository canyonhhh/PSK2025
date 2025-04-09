import { useQuery } from '@tanstack/react-query';
import { fetchRandomNumber } from '../api/requests/randomNumberRequest';
import { keys } from '../api/queryKeyFactory';

function RandomNumber() {
  const { data: number, isLoading } = useQuery({
    queryKey: keys.randomNumber,
    queryFn: fetchRandomNumber,
  });

  return (
        <div>
            <h1>Random Number: {isLoading ? number : 'Loading...'}</h1>
        </div>
  );
}

export default RandomNumber;
