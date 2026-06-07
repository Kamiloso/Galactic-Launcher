import hashlib
import ssl
import urllib3
import requests
import typing

from requests.adapters import HTTPAdapter
from urllib3.connection import HTTPSConnection
from urllib3.connectionpool import HTTPSConnectionPool
from urllib3.poolmanager import PoolManager

from utils.state import State
from utils.const import Const

urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)


def cert_thumbprint() -> str:
    if State.dev_mode:
        return Const.DEV_CERT_THUMBPRINT
    return Const.PRD_CERT_THUMBPRINT


def endpoint() -> str:
    if State.dev_mode:
        return Const.DEV_ENDPOINT
    return Const.PRD_ENDPOINT


class PinnedConnection(HTTPSConnection):
    def connect(self) -> None:
        super().connect()
        
        ssl_sock = typing.cast(ssl.SSLSocket, self.sock)
        cert_der = ssl_sock.getpeercert(binary_form=True)
        
        if not cert_der:
            raise ssl.SSLError("Connection dennied. No certificate provided by the server.")

        fingerprint = hashlib.sha256(cert_der).hexdigest().lower()
        expected = cert_thumbprint().replace(":", "").lower()

        if fingerprint != expected:
            raise ssl.SSLError(f"Certificate pinning error! Expected: {expected}, received: {fingerprint}")


class PinnedConnectionPool(HTTPSConnectionPool):
    ConnectionCls = PinnedConnection  # type: ignore


class PinnedAdapter(HTTPAdapter):
    def init_poolmanager(self, connections, maxsize, block=False, **pool_kwargs):
        self.poolmanager = PoolManager(
            num_pools=connections,
            maxsize=maxsize,
            block=block,
            **pool_kwargs
        )
        self.poolmanager.pool_classes_by_scheme['https'] = PinnedConnectionPool


class HttpClient:
    def __init__(self):
        self.session = requests.Session()
        self.session.verify = False 
        self.session.mount('https://', PinnedAdapter())

    def _build_url(self, path: str) -> str:
        base = endpoint().rstrip('/')
        p = path.lstrip('/')
        return f"{base}/{p}"

    def get(self, path: str, **kwargs) -> requests.Response:
        return self.session.get(self._build_url(path), **kwargs)

    def post(self, path: str, **kwargs) -> requests.Response:

        return self.session.post(self._build_url(path), **kwargs)