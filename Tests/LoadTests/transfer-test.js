import http from 'k6/http';
import { check, sleep } from 'k6';
import { randomIntBetween } from 'https://jslib.k6.io/k6-utils/1.2.0/index.js';

// Конфигурация теста
export const options = {
  stages: [
    { duration: '30s', target: 10 },  // постепенно увеличиваем до 10 пользователей
    { duration: '1m', target: 10 },   // держим 10 пользователей
    { duration: '30s', target: 25 },  // увеличиваем до 25 пользователей
    { duration: '1m', target: 25 },   // держим 25 пользователей
    { duration: '30s', target: 50 },  // увеличиваем до 50 пользователей
    { duration: '1m', target: 50 },   // держим 50 пользователей
    { duration: '30s', target: 0 },   // постепенно уменьшаем до 0
  ],
  thresholds: {
    http_req_duration: ['p(95)<500'], // 95% запросов должны быть быстрее 500ms
    http_req_failed: ['rate<0.01'],   // менее 1% запросов могут завершиться ошибкой
  },
};

// Глобальная переменная для хранения списка счетов
let accounts = [];

// Функция инициализации - выполняется один раз перед тестом
export function setup() {
  console.log('Получение списка счетов...');
  const response = http.get('http://localhost:5241/Accounts');
  
  if (response.status !== 200) {
    throw new Error(`Не удалось получить счета: ${response.status}`);
  }
  
  accounts = response.json();
  console.log(`Получено счетов: ${accounts.length}`);
  
  // Проверяем, что есть достаточно счетов для тестирования
  if (accounts.length < 2) {
    throw new Error('Недостаточно счетов для выполнения переводов');
  }
  
  return { accounts };
}

// Основная функция теста
export default function(data) {
  
    // Используем счета из setup данных
  const availableAccounts = data.accounts || accounts;
  
  if (availableAccounts.length < 2) {
    console.error('Недостаточно счетов для выполнения перевода');
    return;
  }

  // Выбираем два случайных разных счета
  let fromAccount, toAccount;
  
  do {
    const fromIndex = randomIntBetween(0, availableAccounts.length - 1);
    const toIndex = randomIntBetween(0, availableAccounts.length - 1);
    fromAccount = availableAccounts[fromIndex];
    toAccount = availableAccounts[toIndex];
  } while (fromAccount.number === toAccount.number);

  // Подготавливаем тело запроса для перевода
  const transferData = {
    fromAccountNumber: fromAccount.number,
    toAccountNumber: toAccount.number,
    amount: randomIntBetween(1, 100), // Случайная сумма от 1 до 100
    description: `Load test transfer ${Date.now()}`
  };

  const params = {
    headers: {
      'Content-Type': 'application/json',
    },
  };

  // Выполняем запрос на перевод
  const response = http.post(
    'http://localhost:5241/Transfer',
    JSON.stringify(transferData),
    params
  );

  // Проверяем результат
  check(response, {
    'transfer status is 200': (r) => r.status === 200,
    'transfer response time OK': (r) => r.timings.duration < 1000,
    'transfer has valid response': (r) => {
      if (r.status === 200) {
        try {
          const body = r.json();
          return body !== null;
        } catch {
          return false;
        }
      }
      return true;
    },
  });

  // Небольшая пауза между запросами (от 0.5 до 2 секунд)
  sleep(randomIntBetween(0.5, 2));
}

// Функция завершения теста (опционально)
export function teardown(data) {
  console.log('Тестирование завершено');
  console.log(`Всего было доступно счетов: ${data.accounts.length}`);
}