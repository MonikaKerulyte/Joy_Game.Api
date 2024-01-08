import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    insecureSkipTLSVerify: true,
    noConnectionReuse: false,
    stages: [
        // A list of virtual users { target: ..., duration: ... } objects that specify 
        // the target number of VUs to ramp up or down to for a specific period.
        { duration: '1m', target: 1000 }, // simulate ramp-up of traffic from 1 to 100 users over 5 minutes.
        //{ duration: '1m', target: 100 }, // stay at 100 users for 10 minutes
        //{ duration: '1m', target: 0 }, // ramp-down to 0 users
    ],
    thresholds: {
        http_req_failed: ['rate<0.01'], // http errors should be less than 1%
        http_req_duration: ['p(95)<3000'], // 95% of requests should be below 200ms
    },
};

export default function () {
    // Here, we set the endpoint to test.
    const response = http.get('https://localhost:7278/api/v1/game/mode');

    // An assertion
    check(response, {
        'is status 200': (x) => x.status === 200
    });

    sleep(1);
}