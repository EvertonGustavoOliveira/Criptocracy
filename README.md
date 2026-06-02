# Concluido:

* Movimento Player
* Inimigos
* Area 1
* Area 2
* Area 3
* Animacao de esconder
* Camera seguir o player

# Pendências:

## Player:

* Ao pular e tocar em um collider, se ele mantiver a tecla de movimento pressionada em direção a parede, ele fica preso.
* Quando morrer, ele deve possuir um spawnpoint na área que ele está situado.
* Na área 3, ele só pode ir embora pela escotilha da direita se tiver o papel que está na mesa, há na área 3 um sprite da mesa sem papel, mas invísivel.
* Player deve morrer ao tocar atrás do inimigo.
* Player não pode pular enquanto estiver movendo a caixa.

## Audios:

Todos :>

## Áreas

* Todas as áreas devem ter uma imagem de tutorial (área 1 movimento, área 2 empurrar caixa e etc.)
* Área 5 basicamente não ta pronta
* Na área 3 deve ter a lógica do papel (explicado anteriormente no player)
* Colocar a chave em alguma porta (não tem a chave ainda na unity)

## Animações

* Animação do player empurrando a caixa
* Animação da porta

## Extras

* Tela de gameover
* Tela de voce venceu

## Notas

* Tilemap ta todo cagado, perdão
* Se a camera estiver errada, mudar para play maximized em full hd, em teoria assim deve estar correto a camera

# Estruturas importantes da hierarquia:

## Grid fase x (x = numero da area)

Cada area há um grid específico para assim ser possível alterar apenas os tiles de determinada área

## Cameras

A localização e as dimensões das cameras de cada área, o número da camera determina a área que ela está designada

## Limites

Os limites que cada camera pode ver

## Areas de teleporte

Era pra ser aonde o player se teleporta sempre que troca de fase, mas so usei um