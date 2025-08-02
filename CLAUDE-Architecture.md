# CLAUDE-Architecture.md - System Architecture Overview

High-level system overview and core workflows for AutoBot-Enterprise.

## 🏗️ **SYSTEM OVERVIEW**

AutoBot-Enterprise is a comprehensive customs broker automation system built around ASYCUDA (Automated System for Customs Data) operations. It automates customs declarations, trade document processing, inventory management, and regulatory compliance for customs brokers and importers.

### **Key Business Domain**
- **Primary Function**: Customs broker automation and ASYCUDA document processing
- **Core Workflow**: Email-driven document ingestion → OCR/parsing → customs declaration generation → electronic filing
- **Target Users**: Customs brokers, importers, freight forwarders dealing with international trade

### **Technology Stack**
- **.NET Framework 4.8**: Core application framework
- **Entity Framework 6.x**: Data access layer with multiple domain contexts
- **SQL Server**: Primary database (WebSource-AutoBot)
- **WPF + Console Applications**: Multiple application entry points
- **OCR Integration**: Tesseract + AI-powered document extraction (DeepSeek API)
- **Email Processing**: IMAP-based automated email monitoring

## 🔄 **CORE WORKFLOW ARCHITECTURE**

### **1. Email-Driven Processing Pipeline**
```
Email Monitoring → Attachment Extraction → Document Classification → Processing Actions
     ↓                    ↓                       ↓                      ↓
IMAP Client        FileTypeManager         FileActions         Database Updates
```

**Components:**
- **EmailDownloader**: Automated email monitoring and attachment processing
- **FileTypeManager**: Document type identification and classification  
- **Action-Based Processing**: Database-configured workflows with C# implementations
- **SessionActions**: Scheduled and triggered processing workflows

### **2. Document Processing Architecture**
```
PDF/Document Input → OCR/Text Extraction → Data Parsing → Business Logic → ASYCUDA Output
        ↓                   ↓                  ↓             ↓              ↓
   FileUtils.cs        PDFUtils.cs     EntryData/Details   Utils.cs   AsycudaDocument
```

**Key Processing Steps:**
1. **Document Import**: PDF processing and OCR text extraction
2. **Data Extraction**: Invoice parsing with pattern matching and AI assistance
3. **Business Logic**: Customs calculations, tariff classification, compliance validation
4. **Document Generation**: ASYCUDA XML creation for electronic filing
5. **Quality Assurance**: TODO system for exception handling and manual review

### **3. OCR Correction Service Pipeline** 
```
Raw OCR Text → Error Detection → AI Correction → Template Creation → Database Updates
     ↓              ↓               ↓              ↓                ↓
OCRCorrectionService → DeepSeek API → Pattern Generation → Template Storage → Learning System
```

**Advanced OCR Components:**
- **Error Detection**: Multi-field omission and format error analysis
- **AI Integration**: DeepSeek API for intelligent pattern generation
- **Template System**: Dynamic template creation for new invoice types
- **Learning System**: Pattern reuse and continuous improvement
- **Fallback Control**: Configurable fail-fast vs. graceful degradation

## 🏢 **APPLICATION ARCHITECTURE**

### **Multiple Entry Points**
- **AutoBot1** (Console): Main batch processing engine
- **WaterNut** (WPF): Interactive document management interface
- **WCFConsoleHost** (Service): Background service for automated processing
- **EmailDownloader** (Console): Dedicated email processing service

### **Core Libraries**
- **AutoBotUtilities**: Main business logic and utility functions
- **CoreEntities**: Primary data model and Entity Framework context
- **WaterNut.Business.Services**: Service layer with business logic
- **InvoiceReader**: OCR and document processing pipeline
- **TrackableEntities**: Change tracking for distributed data synchronization

### **Domain-Specific Data Contexts**
- **AllocationDS/QS**: Inventory allocation and sales processing
- **DocumentDS/QS**: Document management and workflow tracking
- **EntryDataDS/QS**: Import data processing and customs declarations
- **InventoryDS/QS**: Product catalog and tariff classification
- **SalesDataQS**: Sales transaction processing and reporting

## 📊 **DATA ARCHITECTURE**

### **Entity Framework Structure**
- **CoreEntities**: Main business entities and database context
- **Multiple EDMX Models**: Domain-specific data models for different business areas
- **T4 Templates**: Code generation for entities, mappings, and service layers
- **Multiple Database Contexts**: Separated concerns for different business domains

### **Key Business Entities**
- **AsycudaDocument**: Individual customs declarations/entries
- **AsycudaDocumentSet**: Container for grouped customs documents
- **EntryData/EntryDataDetails**: Parsed invoice and trade document data
- **InventoryItems**: Master product catalog with tariff classifications
- **ApplicationSettings**: Per-company configuration management
- **FileTypes/FileActions**: Configurable document processing workflows

### **Database Integration Patterns**
- **Action-Based Configuration**: Database-driven workflow definitions
- **TODO Views**: Database views for identifying incomplete/problematic entries
- **Multi-Tenant Support**: Company-specific processing and configuration
- **Audit Trails**: Comprehensive logging of processing actions and changes

## ⚙️ **PROCESSING ARCHITECTURE**

### **Action-Based Processing Pattern**
The system uses a database-driven action pattern where workflows are configured in the database and mapped to C# implementations:

```csharp
// FileActions Dictionary (Utils.cs)
public static Dictionary<string, Func<FileInfo, bool>> FileActions = new Dictionary<string, Func<FileInfo, bool>>
{
    { "ImportPDF", ImportPDF },
    { "ProcessInvoice", ProcessInvoice },
    { "CreateAsycudaDocument", CreateAsycudaDocument },
    // ... database-configured actions mapped to C# methods
};
```

**Benefits:**
- **Runtime Configuration**: Change processing without code changes
- **Multi-Client Support**: Different workflows per business entity
- **Audit Trail**: Database tracking of all processing actions
- **Exception Handling**: Systematic error reporting and manual intervention

### **OCR Integration Architecture**
```
Traditional OCR (Tesseract) ←→ AI-Powered OCR (DeepSeek) ←→ Template System
                ↓                         ↓                       ↓
        Pattern Matching            Dynamic Correction        Reusable Templates
                ↓                         ↓                       ↓
           EntryData              Improved EntryData       Future Processing
```

**Hybrid Approach:**
- **Traditional OCR**: Fast, reliable for standard document formats
- **AI-Powered Correction**: Handles complex layouts and new document types
- **Template Learning**: Captures successful patterns for reuse
- **Fallback Strategy**: Graceful degradation when AI processing fails

## 🔧 **INTEGRATION ARCHITECTURE**

### **External Dependencies**
- **ASYCUDA System**: Electronic customs declaration filing
- **Email Servers**: IMAP-based document ingestion
- **OCR Engines**: Tesseract for text extraction, DeepSeek for intelligent processing
- **SQL Server**: Primary data storage and business logic execution

### **Internal Integration Patterns**
- **Event-Driven Processing**: Email triggers → File processing → Database updates
- **Pipeline Architecture**: Modular processing steps with clear interfaces
- **Dependency Injection**: Logger and configuration propagation
- **Configuration Management**: Database-driven + file-based configuration

### **Service Communication**
- **WCF Services**: Inter-application communication (legacy)
- **Database Queues**: Asynchronous processing coordination
- **File System**: Document storage and processing coordination
- **Shared Libraries**: Common business logic across applications

## 🛡️ **QUALITY ASSURANCE ARCHITECTURE**

### **Exception Handling Strategy**
- **TODO System**: Database views identify incomplete operations
- **Automated Reporting**: Exception reports for manual review
- **Stakeholder Notifications**: Email alerts to brokers and clients
- **Manual Intervention Points**: Human oversight for complex cases

### **Data Validation Framework**
- **Business Rule Engine**: Caribbean customs compliance validation
- **Field Validation**: Invoice data completeness and format checking
- **Cross-Reference Validation**: Inventory and tariff code verification
- **Financial Validation**: Balance calculations and reconciliation

### **Testing Architecture**
- **Unit Tests**: Individual component validation
- **Integration Tests**: End-to-end workflow validation  
- **OCR-Specific Tests**: Document processing accuracy validation
- **Performance Tests**: Large-volume processing validation

## 📈 **SCALABILITY CONSIDERATIONS**

### **Processing Volume**
- **Batch Processing**: Designed for high-volume automated processing
- **Parallel Processing**: Multiple workers for independent document processing
- **Database Optimization**: Indexed views and stored procedures for performance
- **Caching Strategy**: Template and configuration caching for repeated operations

### **Extensibility Patterns**
- **Plugin Architecture**: New document types via database configuration
- **Template System**: Dynamic invoice format support
- **API Integration**: Extensible external service integration
- **Multi-Tenant Design**: Support for multiple companies and configurations

---

*This architecture overview provides the foundation for understanding AutoBot-Enterprise's comprehensive customs automation capabilities.*