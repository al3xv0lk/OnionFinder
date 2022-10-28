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
O programa baixa, instala, configura e otimiza o Tor de forma automatizada e o utiliza como proxy para realizar buscas no Web Search Ahmia, todos os links obtidos então são testados de forma paralela e assíncrona.
## Guia de uso
1. Baixe o projeto na pasta desejada e extraia.
2. Instale as dependências no arquivo `OnionFinder.csproj`.
3. Utilize o comando `dotnet publish -r linux-x64` para gerar o executavel.
4. Abra o terminal na pasta do executável criado e `./OnionFinder` para iniciar o programa, durante a primeira inicialização aguarde a devida instalação e configuração automática.
5. Digite o termo que gostaria de pesquisar e aguarde os resultados.
6. Copie o link desejado e cole no Tor-Browser.
## Atenção
1. O Tor-Browser será aberto automaticamente sempre que o programa for executado, ele é necessário para manter o proxy ligado e também facilita as pesquisas.
2. Caso o Tor-Browser ou o OnionFinder não consigam conectar a rede, feche o Tor, feche o OnionFinder e execute o mesmo novamente
3. Mesmo otimizado, a velocidade das buscas pode oscilar devido aos nodes do próprio Tor e dos servidores do Ahmia(web search utilizado pelo OnionFinder).
## Suporte
Linux
## Roadmap
- Suporte Windows
- Otimização das buscas
