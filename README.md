<img
  src="https://user-images.githubusercontent.com/59628368/197887817-0562d295-b7e7-4ce6-a61b-0a524a18bf85.png"
  alt="Alt text"
  title="Optional title"
  style="display: inline-block; margin: 0 auto; width: 350px">
<img
  src="https://user-images.githubusercontent.com/59628368/197893103-8cd20793-097b-4400-aa46-92707de0f641.png"
  alt="Alt text"
  title="Optional title"
  style="display: inline-block; margin: 0 auto; width: 350px">
## Sobre o projeto
O OnionFinder é um auxiliar de buscas na rede Tor(Deep Web), com ele você realiza pesquisas e recebe links que realmente estão online, pois ele analisa todos os links antes de retornar os resultados, evitando perda de tempo por conta de links quebrados.
## Como funciona
O OnionFinder utiliza o Tor como proxy para realizar buscas no Web Search Ahmia, todos os links obtidos são testados de forma paralela e assíncrona.
## Guia de uso
1. Baixe e instale o Tor Browser no site oficial: https://www.torproject.org/download/
2. Baixe o executável do OnionFinder dentro da pasta "Releases"(ou compile com o source)
3. Copie o executável para a pasta raiz do Tor
4. Adicione a linha "SocksPort 127.0.0.1:9050" ao arquivo "torrc", localizado em: "TorBrowser\Data\Tor"
5. Inicie o Tor e conecte.
6. LINUX: Abra o terminal na pasta do executável e `./OnionFinder` para iniciar o programa. WINDOWS: Abra o OnionFinder normalmente.
7. Digite o termo que gostaria de pesquisar e aguarde os resultados.
8. Copie o link desejado e cole no Tor-Browser.
## Atenção
1. O Tor-Browser é necessário para manter o proxy ligado e também facilita as pesquisas.
2. Caso o Tor-Browser ou o OnionFinder não consigam conectar a rede, reinicie o Tor e reinicie o OnionFinder.
3. Mesmo otimizado, a velocidade das buscas pode oscilar devido aos nodes do próprio Tor e dos servidores do Ahmia(web search utilizado pelo OnionFinder).
## Suporte
Windows e Linux
