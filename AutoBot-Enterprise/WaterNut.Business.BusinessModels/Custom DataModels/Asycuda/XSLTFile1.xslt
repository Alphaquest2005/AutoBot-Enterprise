<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" Exclude-result-prefixes="msxsl"
>
    <xsl:output method="xml" indent="yes"/>

  <xsl:template match="*">
    <xsl:param name="parentElm">
      <xsl:value-of select="name(..)" />
    </xsl:param>
    <xsl:choose>
      <xsl:when test="local-name() = 'Contact'">
        <xsl:element name="{concat('Contact',$parentElm)}">
          <xsl:apply-templates select="@* | node()" />
        </xsl:element>
      </xsl:when>
      <xsl:otherwise>
        <xsl:element name="{local-name()}">
          <xsl:copy-of select="@*" />
          <xsl:apply-templates select="@* | node()" />
        </xsl:element>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>
