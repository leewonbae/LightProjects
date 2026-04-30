//using System;
//using System.Diagnostics;
//using System.Net.Http;
//using System.Threading;
//using System.Threading.Tasks;
//using Xunit;

//namespace pray_server.Tests.PerformanceTests
//{
//    /// <summary>
//    /// 실제 HTTP 요청 기반 부하 테스트
//    /// 테스트 서버가 실행 중인 상태에서 실행
//    /// 
//    /// 실행 방법:
//    /// 1. pray-server 실행 (https://localhost:7121 또는 해당 포트)
//    /// 2. 아래 테스트 실행
//    /// </summary>
//    public class HttpLoadTest
//    {
//        private const string BASE_URL = "https://localhost:7121";
//        private const int TOTAL_REQUESTS = 100;  // 테스트용 100개 (전체는 너무 오래 걸림)

//        [Fact(Skip = "실제 테스트 시에만 실행. 서버가 실행 중이어야 함")]
//        public async Task LoadTest_ConcurrentRequests_100()
//        {
//            using var httpClient = new HttpClient();
//            httpClient.Timeout = TimeSpan.FromSeconds(30);

//            var stopwatch = Stopwatch.StartNew();
//            var tasks = new Task[TOTAL_REQUESTS];
//            var completedRequests = 0;
//            var failedRequests = 0;

//            Console.WriteLine("=== HTTP 부하 테스트 시작 ===");
//            Console.WriteLine($"기준 URL: {BASE_URL}");
//            Console.WriteLine($"총 요청: {TOTAL_REQUESTS}");
//            Console.WriteLine($"동시성: 모두 한 번에 시작");
//            Console.WriteLine();

//            // 모든 요청을 동시에 시작
//            for (int i = 0; i < TOTAL_REQUESTS; i++)
//            {
//                int requestNumber = i;
//                tasks[i] = SendRequestAsync(httpClient, requestNumber, () =>
//                {
//                    Interlocked.Increment(ref completedRequests);
//                });
//            }

//            try
//            {
//                await Task.WhenAll(tasks);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"요청 실패: {ex.Message}");
//                failedRequests = TOTAL_REQUESTS - completedRequests;
//            }

//            stopwatch.Stop();

//            // 결과 출력
//            Console.WriteLine("=== 테스트 결과 ===");
//            Console.WriteLine($"완료된 요청: {completedRequests}");
//            Console.WriteLine($"실패한 요청: {failedRequests}");
//            Console.WriteLine($"총 소요 시간: {stopwatch.ElapsedMilliseconds}ms");
//            Console.WriteLine($"평균 응답 시간: {stopwatch.ElapsedMilliseconds / (double)completedRequests:N2}ms");
//            Console.WriteLine($"초당 처리량: {completedRequests / stopwatch.Elapsed.TotalSeconds:N0} req/sec");
//            Console.WriteLine();

//            Assert.True(completedRequests > TOTAL_REQUESTS * 0.9, 
//                "90% 이상의 요청이 완료되어야 함");
//        }

//        private async Task SendRequestAsync(HttpClient client, int requestNumber, Action onComplete)
//        {
//            try
//            {
//                var json = @"{""userId"": 1, ""platformType"": 1, ""platformToken"": ""test_token_"" + requestNumber + @"""}";
//                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

//                var response = await client.PostAsync(
//                    $"{BASE_URL}/action?packetName=ReqIdentify",
//                    content);

//                response.EnsureSuccessStatusCode();
//                onComplete?.Invoke();

//                if (requestNumber % 10 == 0)
//                {
//                    Console.WriteLine($"요청 {requestNumber} 완료");
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"요청 {requestNumber} 실패: {ex.Message}");
//            }
//        }
//    }

//    /// <summary>
//    /// 스트레스 테스트 - 점진적으로 부하 증가
//    /// </summary>
//    public class StressTest
//    {
//        [Fact(Skip = "실제 테스트 시에만 실행. 서버가 실행 중이어야 함")]
//        public async Task StressTest_GradualLoad()
//        {
//            Console.WriteLine("=== 스트레스 테스트 (점진적 부하 증가) ===");
//            Console.WriteLine();

//            var testLevels = new[] { 10, 50, 100, 200, 500 };
//            using var httpClient = new HttpClient();
//            httpClient.Timeout = TimeSpan.FromSeconds(60);

//            foreach (var level in testLevels)
//            {
//                Console.WriteLine($"테스트 레벨: {level}개 동시 요청");
//                var stopwatch = Stopwatch.StartNew();
//                var tasks = new Task[level];

//                for (int i = 0; i < level; i++)
//                {
//                    int requestNumber = i;
//                    tasks[i] = SendIdentifyRequestAsync(httpClient, requestNumber);
//                }

//                try
//                {
//                    await Task.WhenAll(tasks);
//                    stopwatch.Stop();

//                    Console.WriteLine($"  완료 시간: {stopwatch.ElapsedMilliseconds}ms");
//                    Console.WriteLine($"  평균 응답: {stopwatch.ElapsedMilliseconds / (double)level:N2}ms");
//                    Console.WriteLine($"  처리량: {level / stopwatch.Elapsed.TotalSeconds:N0} req/sec");
//                    Console.WriteLine();

//                    // 다음 레벨 전에 잠시 대기
//                    await Task.Delay(1000);
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"  실패: {ex.Message}");
//                    break;
//                }
//            }
//        }

//        private async Task SendIdentifyRequestAsync(HttpClient client, int requestNumber)
//        {
//            try
//            {
//                var json = @"{""userId"": 1, ""platformType"": 1, ""platformToken"": ""test_"" + requestNumber + @"""}";
//                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

//                await client.PostAsync(
//                    "https://localhost:7121/action?packetName=ReqIdentify",
//                    content);
//            }
//            catch
//            {
//                // 요청 실패 무시
//            }
//        }
//    }

//    /// <summary>
//    /// 지속성 테스트 - 오래 실행하여 메모리 누수 등 확인
//    /// </summary>
//    public class EnduranceTest
//    {
//        [Fact(Skip = "실제 테스트 시에만 실행. 시간이 오래 걸림")]
//        public async Task EnduranceTest_LongRunning()
//        {
//            Console.WriteLine("=== 지속성 테스트 (10분간 연속 요청) ===");
//            Console.WriteLine();

//            using var httpClient = new HttpClient();
//            httpClient.Timeout = TimeSpan.FromSeconds(30);

//            var totalRequests = 0;
//            var failedRequests = 0;
//            var stopwatch = Stopwatch.StartNew();
//            var testDuration = TimeSpan.FromMinutes(10);

//            while (stopwatch.Elapsed < testDuration)
//            {
//                var batchSize = 50;
//                var tasks = new Task[batchSize];

//                for (int i = 0; i < batchSize; i++)
//                {
//                    int requestNumber = totalRequests + i;
//                    tasks[i] = SendIdentifyRequestAsync(httpClient, requestNumber);
//                }

//                try
//                {
//                    await Task.WhenAll(tasks);
//                    totalRequests += batchSize;
//                }
//                catch
//                {
//                    failedRequests += batchSize;
//                }

//                if (totalRequests % 500 == 0)
//                {
//                    var memoryUsage = GC.GetTotalMemory(false) / (1024 * 1024);
//                    Console.WriteLine($"진행: {totalRequests} 요청, 메모리: {memoryUsage}MB");
//                }
//            }

//            stopwatch.Stop();

//            Console.WriteLine();
//            Console.WriteLine("=== 지속성 테스트 결과 ===");
//            Console.WriteLine($"총 요청: {totalRequests}");
//            Console.WriteLine($"실패: {failedRequests}");
//            Console.WriteLine($"소요 시간: {stopwatch.Elapsed.TotalMinutes:N2}분");
//            Console.WriteLine($"초당 처리량: {totalRequests / stopwatch.Elapsed.TotalSeconds:N0} req/sec");
//            Console.WriteLine($"성공률: {(totalRequests - failedRequests) / (double)totalRequests * 100:N2}%");
//        }

//        private async Task SendIdentifyRequestAsync(HttpClient client, int requestNumber)
//        {
//            try
//            {
//                var json = @"{""userId"": 1, ""platformType"": 1, ""platformToken"": ""test_"" + requestNumber + @"""}";
//                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

//                await client.PostAsync(
//                    "https://localhost:7121/action?packetName=ReqIdentify",
//                    content);
//            }
//            catch
//            {
//                // 실패 무시
//            }
//        }
//    }
//}
