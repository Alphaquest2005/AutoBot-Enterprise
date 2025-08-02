# CLAUDE.md Sync Fix Instructions

## 🏠 WORKTREE ENVIRONMENT DETECTION

### **🎯 Current Environment Commands**
```bash
# Always run this first to identify your current environment
echo "Current Environment: $(pwd)"
echo "Current Branch: $(git branch --show-current)"
echo "Repository Root: $(git rev-parse --show-toplevel)"
git worktree list
```

**Available Environments**:
- **Main Repository**: `AutoBot-Enterprise` (primary development)
- **Alpha Worktree**: `AutoBot-Enterprise-alpha` (experimental work)
- **Beta Worktree**: `AutoBot-Enterprise-beta` (baseline comparison)

---

## 🚨 CRITICAL ISSUE IDENTIFIED

The `claude-folder-sync.sh` script is overwriting CLAUDE.md content instead of preserving it. This causes loss of SuperClaude configuration and other important content.

## 🔧 IMMEDIATE FIX NEEDED

Replace the `sync_claude_md()` function in `/home/joseph/.claude/claude-folder-sync.sh` with this improved version:

```bash
# Smart CLAUDE.md sync with content preservation
sync_claude_md() {
    local global_claude="${GLOBAL_CLAUDE}/CLAUDE.md"
    local project_claude="${PROJECT_CLAUDE}/CLAUDE.md"
    local global_dump="${GLOBAL_CLAUDE}/claude-dump.md"
    local project_dump="${PROJECT_CLAUDE}/claude-dump.md"
    
    log "INFO" "Performing intelligent CLAUDE.md synchronization with content preservation..."
    
    if [[ $DRY_RUN == true ]]; then
        log "DRY-RUN" "Would analyze and sync CLAUDE.md files with content dump preservation"
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
    
    # Determine sync direction and preserve content that would be overwritten
    if [[ $global_mtime -gt $project_mtime ]]; then
        log "INFO" "Global CLAUDE.md is newer - saving project version to dump and syncing"
        # Save project content to dump before overwriting
        if [[ -s "$project_claude" ]]; then
            local timestamp=$(date '+%Y-%m-%d %H:%M:%S')
            echo "# CLAUDE.md Content Preserved - $timestamp" > "$project_dump"
            echo "# This content was in project CLAUDE.md before sync from global" >> "$project_dump"
            echo "# Location: $project_claude" >> "$project_dump"
            echo "" >> "$project_dump"
            cat "$project_claude" >> "$project_dump"
            log "INFO" "Project CLAUDE.md content saved to: $project_dump"
        fi
        cp -p "$global_claude" "$project_claude"
    elif [[ $project_mtime -gt $global_mtime ]]; then
        log "INFO" "Project CLAUDE.md is newer - saving global version to dump and syncing"
        # Save global content to dump before overwriting
        if [[ -s "$global_claude" ]]; then
            local timestamp=$(date '+%Y-%m-%d %H:%M:%S')
            echo "# CLAUDE.md Content Preserved - $timestamp" > "$global_dump"
            echo "# This content was in global CLAUDE.md before sync from project" >> "$global_dump"
            echo "# Location: $global_claude" >> "$global_dump"
            echo "" >> "$global_dump"
            cat "$global_claude" >> "$global_dump"
            log "INFO" "Global CLAUDE.md content saved to: $global_dump"
        fi
        cp -p "$project_claude" "$global_claude"
    else
        log "WARN" "CLAUDE.md files have same timestamp but different content"
        # Save both versions to dumps for manual resolution
        local timestamp=$(date '+%Y-%m-%d %H:%M:%S')
        echo "# CLAUDE.md Conflict - Global Version - $timestamp" > "$global_dump"
        echo "# Manual resolution needed - files have same timestamp but different content" >> "$global_dump"
        echo "" >> "$global_dump"
        cat "$global_claude" >> "$global_dump"
        
        echo "# CLAUDE.md Conflict - Project Version - $timestamp" > "$project_dump"
        echo "# Manual resolution needed - files have same timestamp but different content" >> "$project_dump"
        echo "" >> "$project_dump"
        cat "$project_claude" >> "$project_dump"
        
        log "WARN" "Both versions saved to dump files for manual resolution: $global_dump, $project_dump"
        return 1
    fi
}
```

## 📋 MANUAL STEPS TO APPLY FIX

1. **Edit the sync script**:
   ```bash
   nano /home/joseph/.claude/claude-folder-sync.sh
   ```

2. **Find the `sync_claude_md()` function** (around line 71)

3. **Replace it completely** with the improved version above

4. **Save and test**:
   ```bash
   cd /home/joseph/.claude
   ./claude-folder-sync.sh --dry-run
   ```

## 🎯 WHAT THIS FIX DOES

- **Preserves content**: Saves any content that would be overwritten to `claude-dump.md`
- **Intelligent merging**: Uses modification time to determine sync direction
- **Content recovery**: Any "lost" content can be recovered from dump files
- **Conflict resolution**: Handles timestamp conflicts by saving both versions

## 🚨 IMMEDIATE ACTION NEEDED

The CLAUDE.md file is currently being emptied repeatedly. Apply this fix immediately to prevent further content loss.

## 📁 RECOVERY FILES

After applying the fix, check these locations for any previously lost content:
- `/home/joseph/.claude/claude-dump.md`
- `$(git rev-parse --show-toplevel)/.claude/claude-dump.md`