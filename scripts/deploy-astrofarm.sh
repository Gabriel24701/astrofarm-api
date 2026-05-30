#!/bin/bash
# ==============================================================================
#                                                    
#   █████╗ ███████╗████████╗██████╗  ██████╗         
#   ██╔══██╗██╔════╝╚══██╔══╝██╔══██╗██╔═══██╗        
#   ███████║███████╗   ██║   ██████╔╝██║   ██║        
#   ██╔══██║╚════██║   ██║   ██╔══██╗██║   ██║        
#   ██║  ██║███████║   ██║   ██║  ██║╚██████╔╝        
#   ╚═╝  ╚═╝╚══════╝   ╚═╝   ╚═╝  ╚═╝ ╚═════╝         
#                                                     
#    ███████╗ █████╗ ██████╗ ███╗   ███╗               
#    ██╔════╝██╔══██╗██╔══██╗████╗ ████║               
#    █████╗  ███████║██████╔╝██╔████╔██║               
#    ██╔══╝  ██╔══██║██╔══██╗██║╚██╔╝██║               
#    ██║     ██║  ██║██║  ██║██║ ╚═╝ ██║               
#    ╚═╝     ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝     ╚═╝               
#                                                     
#    P R O V I S I O N E R   D E   I N F R A  
#                                                     
# ==============================================================================
# PROJETO   : AstroFarm -- Arquitetura Hibrida para Economia Espacial (GS)
# DESCRICAO : Provisioner interativo para Azure VM com Docker, .NET 8 e Oracle DB
# STACK     : .NET 8 API | Oracle DB (gvenzl/oracle-free:slim) | Docker Compose
# VERSAO    : 3.0.0 (Global Solution Edition)
# ==============================================================================

# -- Modo estrito: aborta em erros, variaveis nao definidas e falhas em pipes --
set -euo pipefail

# ==============================================================================
# SECAO 1 -- CONSTANTES GLOBAIS
# ==============================================================================

readonly SCRIPT_VERSION="3.0.0"
readonly TEMP_DIR="$(mktemp -d /tmp/astrofarm-deploy.XXXXXX)"
readonly CLOUD_INIT_FILE="${TEMP_DIR}/cloud-init.yaml"
readonly CLOUD_INIT_SAFE="${TEMP_DIR}/cloud-init-safe.yaml"
readonly VM_OUTPUT_FILE="${TEMP_DIR}/vm-output.json"
readonly LOG_FILE="${TEMP_DIR}/deploy.log"

# Portas expostas pela aplicacao AstroFarm
# 22   -> SSH (acesso admin)
# 1521 -> Oracle Database 
# 8080 -> API .NET 8 (consumida pelo app mobile e sensores IoT)
readonly -a VM_PORTS=(22 1521 8080)
readonly NSG_PRIORITY_BASE=1010

# Dependencias obrigatorias para execucao do script
readonly -a REQUIRED_CMDS=("az" "jq")

# -- Paleta de Cores ANSI -----------------------------------------------------
readonly C_RESET='\033[0m'
readonly C_BOLD='\033[1m'
readonly C_GREEN='\033[1;32m'
readonly C_BLUE='\033[1;34m'
readonly C_CYAN='\033[1;36m'
readonly C_YELLOW='\033[1;33m'
readonly C_RED='\033[1;31m'
readonly C_MAGENTA='\033[1;35m'
readonly C_WHITE='\033[1;37m'
readonly C_DIM='\033[2m'

# -- Simbolos do tema AstroFarm -----------------------------------------------
readonly SYM_FARM="🚜"
readonly SYM_SAT="🛰️"
readonly SYM_LEAF="🌾"
readonly SYM_CLOUD="☁️"
readonly SYM_ROCKET="🚀"
readonly SYM_LOCK="🔒"
readonly SYM_KEY="🔑"
readonly SYM_CHECK="✅"
readonly SYM_WARN="⚠️"
readonly SYM_GLOBE="🌐"

# ==============================================================================
# SECAO 1.5 -- FORCADO ENCODING UTF-8 PARA O AZURE CLI
# ==============================================================================
export PYTHONIOENCODING="utf-8"
export PYTHONUTF8=1
export LANG="en_US.UTF-8"
export LC_ALL="en_US.UTF-8"

# ==============================================================================
# SECAO 2 -- FUNCOES UTILITARIAS (LOGGING E UI)
# ==============================================================================

_log_raw() { echo -e "$1" | tee -a "${LOG_FILE}"; }
log_info()    { _log_raw "  ${C_CYAN}[INFO]${C_RESET}    $1"; }
log_success() { _log_raw "  ${C_GREEN}[OK]${C_RESET}      $1"; }
log_warn()    { _log_raw "  ${C_YELLOW}[AVISO]${C_RESET}   $1"; }
log_error()   { _log_raw "  ${C_RED}[ERRO]${C_RESET}    $1" >&2; }

separator() { echo -e "${C_BLUE}$(printf '%0.s-' {1..64})${C_RESET}"; }

step_header() {
    local -r num="$1" title="$2"
    echo ""
    separator
    echo -e "  ${C_MAGENTA}${C_BOLD}ETAPA ${num}${C_RESET} ${C_DIM}|${C_RESET} ${C_WHITE}${title}${C_RESET}"
    separator
    echo ""
}

show_banner() {
    clear
    echo -e "${C_BLUE}"
    cat << 'BANNER'
  +====================================================+
  |                                                    |
  |   █████╗ ███████╗████████╗██████╗  ██████╗         |
  |  ██╔══██╗██╔════╝╚══██╔══╝██╔══██╗██╔═══██╗        |
  |  ███████║███████╗   ██║   ██████╔╝██║   ██║        |
  |  ██╔══██║╚════██║   ██║   ██╔══██╗██║   ██║        |
  |  ██║  ██║███████║   ██║   ██║  ██║╚██████╔╝        |
  |  ╚═╝  ╚═╝╚══════╝   ╚═╝   ╚═╝  ╚═╝ ╚═════╝         |
  |                                                    |
  |  ███████╗ █████╗ ██████╗ ███╗   ███╗               |
  |  ██╔════╝██╔══██╗██╔══██╗████╗ ████║               |
  |  █████╗  ███████║██████╔╝██╔████╔██║               |
  |  ██╔══╝  ██╔══██║██╔══██╗██║╚██╔╝██║               |
  |  ██║     ██║  ██║██║  ██║██║ ╚═╝ ██║               |
  |  ╚═╝     ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝     ╚═╝               |
  |                                                    |
  |  P R O V I S I O N E R   D E   I N F R A           |
  |                                                    |
  +====================================================+
BANNER
    echo -e "${C_RESET}"
    echo -e "  ${C_CYAN}Plataforma de Monitoramento Agricola via Satelite${C_RESET}"
    echo -e "  ${C_WHITE}Stack: .NET 8 API | Oracle DB | Docker | Azure VM${C_RESET}"
    echo -e "  ${C_DIM}Versao ${SCRIPT_VERSION}  |  Log: ${LOG_FILE}${C_RESET}"
    separator
    echo ""
}
# ==============================================================================
# SECAO 3 -- GERENCIAMENTO DE ERROS E LIMPEZA
# ==============================================================================

cleanup() {
    rm -f "${CLOUD_INIT_FILE}" "${CLOUD_INIT_SAFE}" "${VM_OUTPUT_FILE}" 2>/dev/null || true
    log_info "${C_DIM}Arquivos temporarios removidos. Log preservado em: ${LOG_FILE}${C_RESET}"
}

abort() {
    local -r msg="${1:-"Erro desconhecido."}"
    echo ""
    separator
    log_error "${C_BOLD}Provisionamento abortado!${C_RESET}"
    log_error "${msg}"
    log_error "Consulte o log completo: ${C_BOLD}${LOG_FILE}${C_RESET}"
    separator
    echo ""
    exit 1
}

# ==============================================================================
# SECAO 4 -- VERIFICACAO DE DEPENDENCIAS E AUTENTICACAO AZURE
# ==============================================================================

check_dependencies() {
    step_header "01" "${SYM_CHECK}  Verificando Dependencias do Sistema"

    for cmd in "${REQUIRED_CMDS[@]}"; do
        if command -v "${cmd}" &>/dev/null; then
            log_success "Dependencia encontrada: ${C_BOLD}${cmd}${C_RESET}"
        else
            abort "Dependencia ausente: '${C_BOLD}${cmd}${C_RESET}'."
        fi
    done
}

check_azure_auth() {
    step_header "02" "${SYM_LOCK}  Verificando Autenticacao na Azure"

    if ! az account show &>/dev/null 2>&1; then
        log_warn "Sessao Azure CLI nao encontrada. Iniciando login..."
        echo ""
        if ! az login --output none; then
            abort "Falha na autenticacao. Execute 'az login' manualmente."
        fi
    fi

    local account_name
    account_name=$(az account show --query "user.name" -o tsv)
    log_success "Autenticado como:    ${C_BOLD}${account_name}${C_RESET}"
}

# ==============================================================================
# SECAO 5 -- COLETA E VALIDACAO DE PARAMETROS
# ==============================================================================

_prompt_password() {
    while true; do
        echo -e "\n  ${C_YELLOW}${SYM_LOCK} Senha Admin${C_RESET} ${C_DIM}(>=12 chars, maiuscula, minuscula, numero, especial):${C_RESET}"
        read -rsp "  > " ADMIN_PASSWORD
        echo ""

        echo -e "  ${C_YELLOW}${SYM_LOCK} Confirme a senha:${C_RESET}"
        read -rsp "  > " password_confirm
        echo ""

        if [[ "${ADMIN_PASSWORD}" != "${password_confirm}" ]]; then
            log_warn "As senhas nao coincidem. Tente novamente."
            continue
        fi
        log_success "Senha validada com sucesso."
        break
    done
    export ADMIN_PASSWORD
}

collect_parameters() {
    step_header "03" "${SYM_FARM}  Configurando Parametros do Ambiente AstroFarm"
    echo -e "  ${C_CYAN}Preencha os dados para o provisionamento:${C_RESET}\n"

    read -rp "  $(echo -e "${C_YELLOW}${SYM_CLOUD}  Resource Group${C_RESET} ${C_DIM}(ex: rg-astrofarm-prod):${C_RESET} ")" RG_NAME
    [[ -n "${RG_NAME}" ]] || abort "Resource Group nao pode ser vazio."

    read -rp "  $(echo -e "${C_YELLOW}${SYM_GLOBE}  Regiao Azure${C_RESET}    ${C_DIM}(ex: eastus):${C_RESET}             ")" LOCATION
    [[ -n "${LOCATION}" ]] || abort "Regiao nao pode ser vazia."

    read -rp "  $(echo -e "${C_YELLOW}${SYM_SAT}   Nome da VM${C_RESET}      ${C_DIM}(ex: vm-astrofarm-api):${C_RESET}     ")" VM_NAME
    [[ -n "${VM_NAME}" ]] || abort "Nome da VM nao pode ser vazio."

    read -rp "  $(echo -e "${C_YELLOW}${SYM_KEY}   Usuario Admin:${C_RESET}                                ")" ADMIN_USER
    [[ -n "${ADMIN_USER}" ]] || abort "Usuario Admin nao pode ser vazio."

    _prompt_password
    export RG_NAME LOCATION VM_NAME ADMIN_USER
}

# ==============================================================================
# SECAO 6 -- DESCOBERTA DINAMICA DE SKUs DE VM
# ==============================================================================

select_vm_size() {
    step_header "04" "${SYM_CLOUD}  Selecionando Tamanho da VM"
    log_info "Consultando SKUs disponiveis em '${C_BOLD}${LOCATION}${C_RESET}'..."

    local sku_list
    sku_list=$(az vm list-skus \
        --location "${LOCATION}" \
        --resource-type virtualMachines \
        --query "[?restrictions[0].reasonCode == null && (contains(name, 'Standard_B') || contains(name, 'Standard_D'))].name" \
        -o tsv 2>>"${LOG_FILE}" | head -n 10)

    if [[ -z "${sku_list}" ]]; then
        abort "Nenhuma SKU disponivel em '${LOCATION}'. Verifique a regiao."
    fi

    echo -e "\n  ${C_CYAN}SKUs disponiveis em ${C_BOLD}${LOCATION}${C_RESET}:\n"
    select VM_SIZE in ${sku_list}; do
        if [[ -n "${VM_SIZE}" ]]; then
            log_success "Tamanho selecionado: ${C_BOLD}${VM_SIZE}${C_RESET}"
            break
        fi
    done
    export VM_SIZE
}

# ==============================================================================
# SECAO 7 -- GERACAO DO PAYLOAD CLOUD-INIT
# ==============================================================================

generate_cloud_init() {
    step_header "05" "${SYM_SAT}  Gerando Payload cloud-init"

    cat > "${CLOUD_INIT_FILE}" << CLOUDINIT_HEADER
#cloud-config
package_update: true
package_upgrade: true
packages:
  - apt-transport-https
  - ca-certificates
  - curl
  - gnupg
  - lsb-release
  - git
  - htop

runcmd:
  - curl -fsSL https://get.docker.com -o /tmp/install-docker.sh
  - sh /tmp/install-docker.sh
  - systemctl enable docker
  - systemctl start docker
  - usermod -aG docker ${ADMIN_USER}
  - mkdir -p /opt/astrofarm
  - chown -R ${ADMIN_USER}:${ADMIN_USER} /opt/astrofarm

write_files:
  - path: /etc/motd
    permissions: '0644'
    content: |
      ======================================================
         AstroFarm - VM de Producao (Global Solution)
         Stack: .NET 8 API / Oracle DB / Docker Compose
      ======================================================
CLOUDINIT_HEADER

    log_success "cloud-init.yaml gerado em: ${C_BOLD}${CLOUD_INIT_FILE}${C_RESET}"
}

# ==============================================================================
# SECAO 8 -- PROVISIONAMENTO DA INFRAESTRUTURA AZURE
# ==============================================================================

provision_infrastructure() {
    step_header "06" "${SYM_ROCKET}  Provisionando Infraestrutura na Azure"
    log_info "Criando Resource Group '${C_BOLD}${RG_NAME}${C_RESET}'..."
    az group create --name "${RG_NAME}" --location "${LOCATION}" --output none 2>>"${LOG_FILE}" || abort "Falha no RG."

    log_info "Criando VM '${C_BOLD}${VM_NAME}${C_RESET}' (SKU: ${C_BOLD}${VM_SIZE}${C_RESET}). Isso leva alguns minutos..."
    if ! az vm create \
            --resource-group   "${RG_NAME}" \
            --name             "${VM_NAME}" \
            --image            "Canonical:0001-com-ubuntu-server-jammy:22_04-lts:latest" \
            --size             "${VM_SIZE}" \
            --admin-username   "${ADMIN_USER}" \
            --admin-password   "${ADMIN_PASSWORD}" \
            --authentication-type password \
            --custom-data      "${CLOUD_INIT_FILE}" \
            --output json > "${VM_OUTPUT_FILE}" 2>>"${LOG_FILE}"; then
        abort "Falha ao criar a VM. Verifique o arquivo de log."
    fi
    log_success "VM '${C_BOLD}${VM_NAME}${C_RESET}' criada com sucesso."
}

configure_network_rules() {
    step_header "07" "${SYM_GLOBE}  Configurando Regras de Rede (NSG)"

    local priority=${NSG_PRIORITY_BASE}
    declare -A PORT_LABELS=([22]="SSH" [1521]="Oracle DB" [8080]="API AstroFarm")

    for port in "${VM_PORTS[@]}"; do
        log_info "Liberando porta ${C_BOLD}${port}${C_RESET} -- ${PORT_LABELS[${port}]}..."
        az vm open-port --resource-group "${RG_NAME}" --name "${VM_NAME}" --port "${port}" --priority "${priority}" --output none 2>>"${LOG_FILE}" || log_warn "Erro ao abrir a porta ${port}."
        log_success "Porta ${C_BOLD}${port}${C_RESET} liberada."
        (( priority += 10 ))
    done
}

# ==============================================================================
# SECAO 9 -- RESUMO FINAL DO PROVISIONAMENTO
# ==============================================================================

display_summary() {
    local public_ip
    public_ip=$(jq -r '.publicIpAddress // "N/A"' "${VM_OUTPUT_FILE}")

    echo -e "${C_GREEN}\n  [OK] Infraestrutura Provisionada com Sucesso!${C_RESET}"
    separator
    echo -e "  ${SYM_CLOUD} ${C_WHITE}${C_BOLD}Detalhes da Nuvem${C_RESET}"
    separator
    echo -e "  ${C_CYAN}IP Publico :${C_RESET}  ${C_BOLD}${public_ip}${C_RESET}"
    echo -e "  ${C_CYAN}API URL    :${C_RESET}  http://${public_ip}:8080/swagger"
    echo ""
    separator
    echo -e "  ${SYM_ROCKET} ${C_WHITE}${C_BOLD}Proximos Passos (Workflow Git + Docker)${C_RESET}"
    separator
    echo -e "  Para finalizar o deploy e colocar a API no ar, cole os comandos abaixo no seu terminal:"
    echo ""
    echo -e "  ${C_CYAN}1.${C_RESET} Acesse a VM via SSH:"
    echo -e "     ${C_GREEN}ssh ${ADMIN_USER}@${public_ip}${C_RESET}"
    echo ""
    echo -e "  ${C_CYAN}2.${C_RESET} Clone o seu repositorio dentro da maquina:"
    echo -e "     ${C_GREEN}cd /opt/astrofarm${C_RESET}"
    echo -e "     ${C_GREEN}git clone https://github.com/Gabriel24701/astrofarm-api.git .${C_RESET}  ${C_DIM}#(Nao esqueca o ponto no final!)${C_RESET}"
    echo ""
    echo -e "  ${C_CYAN}3.${C_RESET} Crie o arquivo das variaveis do banco de dados:"
    echo -e "     ${C_GREEN}echo \"ORACLE_ROOT_PASSWORD=SuaSenhaForte123\" > .env${C_RESET}"
    echo -e "     ${C_GREEN}echo \"ORACLE_APP_USER=GS_USER\" >> .env${C_RESET}"
    echo -e "     ${C_GREEN}echo \"ORACLE_APP_PASSWORD=GS_PASSWORD\" >> .env${C_RESET}"
    echo ""
    echo -e "  ${C_CYAN}4.${C_RESET} Suba a arquitetura conteinerizada:"
    echo -e "     ${C_GREEN}docker-compose up -d --build${C_RESET}"
    echo ""
}

main() {
    trap cleanup EXIT
    trap 'echo ""; abort "Script interrompido pelo usuario (CTRL+C)."' INT TERM

    show_banner
    check_dependencies
    check_azure_auth
    collect_parameters
    select_vm_size
    generate_cloud_init
    provision_infrastructure
    configure_network_rules
    display_summary
}

main "$@"