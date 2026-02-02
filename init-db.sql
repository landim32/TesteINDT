-- Script de inicialização do banco de dados PostgreSQL
-- Criado automaticamente ao iniciar o container

-- Criar banco de dados do ContratacaoService se não existir
SELECT 'CREATE DATABASE "ContratacaoServiceDb"'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'ContratacaoServiceDb')\gexec

-- Conectar ao banco PropostaServiceDb
\c PropostaServiceDb

-- Criar schema para o serviço de propostas
CREATE SCHEMA IF NOT EXISTS propostas;

-- Configurar timezone
SET timezone = 'UTC';

-- Extensões úteis
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

-- Comentários
COMMENT ON SCHEMA propostas IS 'Schema para o microserviço de propostas de seguro';

-- Log de inicialização
DO $$
BEGIN
    RAISE NOTICE 'Banco de dados PropostaServiceDb inicializado com sucesso!';
    RAISE NOTICE 'Schema: propostas';
    RAISE NOTICE 'Timestamp: %', NOW();
END $$;

-- Conectar ao banco ContratacaoServiceDb
\c ContratacaoServiceDb

-- Criar schema para o serviço de contratação
CREATE SCHEMA IF NOT EXISTS contratos;

-- Configurar timezone
SET timezone = 'UTC';

-- Extensões úteis
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

-- Comentários
COMMENT ON SCHEMA contratos IS 'Schema para o microserviço de contratação de seguro';

-- Log de inicialização
DO $$
BEGIN
    RAISE NOTICE 'Banco de dados ContratacaoServiceDb inicializado com sucesso!';
    RAISE NOTICE 'Schema: contratos';
    RAISE NOTICE 'Timestamp: %', NOW();
END $$;
