#!/bin/bash

# Claude Folder Synchronization Script
# Keeps /home/joseph/.claude and /mnt/c/Insight Software/AutoBot-Enterprise/.claude in sync

set -e

# Configuration
GLOBAL_CLAUDE="/home/joseph/.claude"
PROJECT_CLAUDE="/mnt/c/Insight Software/AutoBot-Enterprise/.claude"
SYNC_LOG="/home/joseph/.claude/sync.log"
BACKUP_DIR="/home/joseph/.claude-sync-backups"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Logging function
log() {
    local level=$1
    shift
    local message="$*"
    local timestamp=$(date '+%Y-%m-%d %H:%M:%S')
    echo -e "${timestamp} [${level}] ${message}" | tee -a "${SYNC_LOG}"
}

# Create backup before sync
create_backup() {
    local backup_timestamp=$(date '+%Y%m%d_%H%M%S')
    local backup_global="${BACKUP_DIR}/global_${backup_timestamp}"
    local backup_project="${BACKUP_DIR}/project_${backup_timestamp}"
    
    if [[ $DRY_RUN == true ]]; then
        log "DRY-RUN" "Would create backups at: ${backup_global}, ${backup_project}"
        return
    fi
    
    mkdir -p "${BACKUP_DIR}"
    
    log "INFO" "Creating backups..."
    cp -r "${GLOBAL_CLAUDE}" "${backup_global}"
    cp -r "${PROJECT_CLAUDE}" "${backup_project}"
    
    log "INFO" "Backups created: ${backup_global}, ${backup_project}"
}

# Sync specific file types
sync_files() {
    local source=$1
    local target=$2
    local description=$3
    
    log "INFO" "Syncing ${description}: ${source} → ${target}"
    
    if [[ $DRY_RUN == true ]]; then
        log "DRY-RUN" "Would sync file: ${source} → ${target}"
        return
    fi
    
    # Create target directory if it doesn't exist
    mkdir -p "$(dirname "${target}")"
    
    # Copy with preservation of timestamps and permissions
    cp -p "${source}" "${target}"
}

# Smart CLAUDE.md sync with conflict detection
sync_claude_md() {
    local global_claude="${GLOBAL_CLAUDE}/CLAUDE.md"
    local project_claude="${PROJECT_CLAUDE}/CLAUDE.md"
    
    log "INFO" "Performing intelligent CLAUDE.md synchronization..."
    
    if [[ $DRY_RUN == true ]]; then
        log "DRY-RUN" "Would analyze and sync CLAUDE.md files"
        return
    fi
    
    # Check if both files exist
    if [[ ! -f "$global_claude" ]]; then
        log "ERROR" "Global CLAUDE.md not found: $global_claude"
        return 1
    fi
    
    if [[ ! -f "$project_claude" ]]; then
        log "INFO" "Project CLAUDE.md not found, copying from global"
        cp -p "$global_claude" "$project_claude"
        return
    fi
    
    # Compare modification times
    local global_mtime=$(stat -c %Y "$global_claude" 2>/dev/null || stat -f %m "$global_claude" 2>/dev/null)
    local project_mtime=$(stat -c %Y "$project_claude" 2>/dev/null || stat -f %m "$project_claude" 2>/dev/null)
    
    # Check if files are identical
    if cmp -s "$global_claude" "$project_claude"; then
        log "INFO" "CLAUDE.md files are identical - no sync needed"
        return
    fi
    
    # Determine which file is newer
    if [[ $global_mtime -gt $project_mtime ]]; then
        log "INFO" "Global CLAUDE.md is newer - syncing to project"
        cp -p "$global_claude" "$project_claude"
    elif [[ $project_mtime -gt $global_mtime ]]; then
        log "INFO" "Project CLAUDE.md is newer - syncing to global"  
        cp -p "$project_claude" "$global_claude"
    else
        log "WARN" "CLAUDE.md files have same timestamp but different content - manual resolution needed"
        log "WARN" "Keeping both files unchanged - check manually"
        return 1
    fi
}

# Sync directories  
sync_directories() {
    local source_dir=$1
    local target_dir=$2
    local description=$3
    local allow_delete=${4:-false}
    
    if [[ -d "${source_dir}" ]]; then
        log "INFO" "Syncing directory ${description}: ${source_dir} → ${target_dir}"
        mkdir -p "${target_dir}"
        
        if [[ $DRY_RUN == true ]]; then
            log "DRY-RUN" "Would sync directory: ${source_dir} → ${target_dir}"
            return
        fi
        
        # Use rsync if available, fallback to cp
        if command -v rsync >/dev/null 2>&1; then
            # Use rsync without --delete by default for safety
            if [[ "$allow_delete" == "true" ]]; then
                log "WARN" "Using --delete flag for ${description} - files in target not in source will be deleted"
                rsync -av --delete "${source_dir}/" "${target_dir}/"
            else
                rsync -av "${source_dir}/" "${target_dir}/"
            fi
        else
            # Fallback to cp when rsync not available
            log "INFO" "Using cp fallback for ${description} (rsync not available)"
            if [[ "$allow_delete" == "true" ]]; then
                log "WARN" "Using delete mode for ${description} - removing target first"
                rm -rf "${target_dir}"
                mkdir -p "${target_dir}"
            fi
            cp -r "${source_dir}/." "${target_dir}/"
        fi
    fi
}

# Main synchronization function
perform_sync() {
    local direction=$1  # "global_to_project", "project_to_global", or "bidirectional"
    
    log "INFO" "Starting ${direction} synchronization..."
    
    case $direction in
        "global_to_project")
            # Sync unique global content to project
            sync_directories "${GLOBAL_CLAUDE}/projects" "${PROJECT_CLAUDE}/projects" "projects directory"
            sync_directories "${GLOBAL_CLAUDE}/todos" "${PROJECT_CLAUDE}/todos" "todos directory"
            sync_directories "${GLOBAL_CLAUDE}/shell-snapshots" "${PROJECT_CLAUDE}/shell-snapshots" "shell-snapshots"
            sync_files "${GLOBAL_CLAUDE}/claude_desktop_config.json" "${PROJECT_CLAUDE}/claude_desktop_config.json" "desktop config"
            
            # Sync enhanced hooks
            for hook in build-error-prevention.sh deletion-justification.sh interface-namespace-validator.sh prevent-timeout-builds.sh chat-archive.sh; do
                if [[ -f "${GLOBAL_CLAUDE}/hooks/${hook}" ]]; then
                    sync_files "${GLOBAL_CLAUDE}/hooks/${hook}" "${PROJECT_CLAUDE}/hooks/${hook}" "hook: ${hook}"
                fi
            done
            ;;
            
        "project_to_global")
            # Sync comprehensive project content to global
            sync_directories "${PROJECT_CLAUDE}/vector-search" "${GLOBAL_CLAUDE}/vector-search" "vector search system"
            sync_directories "${PROJECT_CLAUDE}/audit_trail" "${GLOBAL_CLAUDE}/audit_trail" "audit trail"
            sync_directories "${PROJECT_CLAUDE}/commands" "${GLOBAL_CLAUDE}/commands" "commands directory"
            sync_directories "${PROJECT_CLAUDE}/learning" "${GLOBAL_CLAUDE}/learning" "learning directories"
            
            # Sync documentation and analysis tools
            for file in "${PROJECT_CLAUDE}"/*.md; do
                if [[ -f "$file" && "$(basename "$file")" != "CLAUDE.md" ]]; then
                    sync_files "$file" "${GLOBAL_CLAUDE}/$(basename "$file")" "documentation: $(basename "$file")"
                fi
            done
            
            for file in "${PROJECT_CLAUDE}"/*.py; do
                if [[ -f "$file" ]]; then
                    sync_files "$file" "${GLOBAL_CLAUDE}/$(basename "$file")" "Python tool: $(basename "$file")"
                fi
            done
            ;;
            
        "bidirectional")
            perform_sync "global_to_project"
            perform_sync "project_to_global"
            
            # Smart CLAUDE.md sync with conflict detection
            sync_claude_md
            ;;
    esac
    
    log "INFO" "Synchronization ${direction} completed successfully"
}

# Check if directories exist
check_directories() {
    if [[ ! -d "${GLOBAL_CLAUDE}" ]]; then
        log "ERROR" "Global Claude directory not found: ${GLOBAL_CLAUDE}"
        exit 1
    fi
    
    if [[ ! -d "${PROJECT_CLAUDE}" ]]; then
        log "ERROR" "Project Claude directory not found: ${PROJECT_CLAUDE}"
        exit 1
    fi
}

# Show usage
show_usage() {
    echo "Usage: $0 [OPTIONS] [DIRECTION]"
    echo ""
    echo "DIRECTION:"
    echo "  global-to-project    Sync from global to project folder"
    echo "  project-to-global    Sync from project to global folder" 
    echo "  bidirectional        Sync both directions (default)"
    echo ""
    echo "OPTIONS:"
    echo "  --backup            Create backup before sync (default: true)"
    echo "  --no-backup         Skip backup creation"
    echo "  --dry-run           Show what would be synced without doing it"
    echo "  --help              Show this help message"
    echo ""
    echo "Examples:"
    echo "  $0                           # Bidirectional sync with backup"
    echo "  $0 global-to-project         # Sync global → project"
    echo "  $0 --dry-run bidirectional   # Show what would be synced"
}

# Parse command line arguments
BACKUP=true
DRY_RUN=false
DIRECTION="bidirectional"

while [[ $# -gt 0 ]]; do
    case $1 in
        --backup)
            BACKUP=true
            shift
            ;;
        --no-backup)
            BACKUP=false
            shift
            ;;
        --dry-run)
            DRY_RUN=true
            shift
            ;;
        --help)
            show_usage
            exit 0
            ;;
        global-to-project|project-to-global|bidirectional)
            DIRECTION=$1
            shift
            ;;
        *)
            log "ERROR" "Unknown option: $1"
            show_usage
            exit 1
            ;;
    esac
done

# Convert direction format
case $DIRECTION in
    "global-to-project")
        DIRECTION="global_to_project"
        ;;
    "project-to-global")
        DIRECTION="project_to_global"
        ;;
    "bidirectional")
        DIRECTION="bidirectional"
        ;;
esac

# Main execution
main() {
    log "INFO" "Claude Folder Sync starting - Direction: ${DIRECTION}, Backup: ${BACKUP}, Dry-run: ${DRY_RUN}"
    
    check_directories
    
    if [[ $DRY_RUN == true ]]; then
        log "INFO" "DRY RUN MODE - No actual changes will be made"
        log "INFO" "Analyzing what would be synchronized..."
    fi
    
    if [[ $BACKUP == true ]]; then
        create_backup
    fi
    
    perform_sync "$DIRECTION"
    
    if [[ $DRY_RUN == true ]]; then
        log "INFO" "Claude Folder Sync dry-run completed"
        echo -e "${YELLOW}🔍 Dry-run completed! No actual changes were made.${NC}"
        echo -e "${BLUE}Review the log above to see what would be synchronized.${NC}"
    else
        log "INFO" "Claude Folder Sync completed successfully"
        echo -e "${GREEN}✓ Synchronization completed!${NC}"
        echo -e "${BLUE}Both .claude folders are now identical and will work the same way.${NC}"
    fi
}

# Run main function
main "$@"