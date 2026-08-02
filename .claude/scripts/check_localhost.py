#!/usr/bin/env python3
"""
Check HTTPS localhost endpoint reachability.
Usage: python check_localhost.py https://localhost:PORT
Exits with code 0 on success, non-zero on failure.
"""
import sys
import ssl
from urllib.request import urlopen, Request
from urllib.error import URLError, HTTPError


def check(url, timeout=5):
    try:
        req = Request(url, headers={"User-Agent": "agent-repo-hook-check/1.0"})
        ctx = ssl.create_default_context()
        # allow self-signed certs for localhost
        ctx.check_hostname = False
        ctx.verify_mode = ssl.CERT_NONE
        with urlopen(req, timeout=timeout, context=ctx) as resp:
            print(f"{url} -> {resp.status} {resp.reason}")
            return 0
    except HTTPError as e:
        print(f"HTTP error: {e.code} {e.reason}")
        return 2
    except URLError as e:
        print(f"URL error: {e}")
        return 3
    except Exception as e:
        print(f"Error: {e}")
        return 4


if __name__ == '__main__':
    if len(sys.argv) < 2:
        print('Usage: python check_localhost.py https://localhost:PORT')
        sys.exit(1)
    url = sys.argv[1]
    code = check(url)
    sys.exit(code)
