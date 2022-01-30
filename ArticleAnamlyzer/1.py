import sys
import json
import base64

from natasha import (
    Segmenter,
    MorphVocab,

    NewsEmbedding,
    NewsMorphTagger,
    NewsSyntaxParser,
    NewsNERTagger,
    LOC,
    PER,

    AddrExtractor,
    DatesExtractor,
    NamesExtractor,

    Doc
)


def analise(text):
    segmenter = Segmenter()
    morph_vocab = MorphVocab()

    emb = NewsEmbedding()
    morph_tagger = NewsMorphTagger(emb)
    syntax_parser = NewsSyntaxParser(emb)
    ner_tagger = NewsNERTagger(emb)

    names_extractor = NamesExtractor(morph_vocab)

    doc = Doc(text)
    doc.segment(segmenter)
    doc.tag_morph(morph_tagger)

    for token in doc.tokens:
        token.lemmatize(morph_vocab)

    doc.tag_ner(ner_tagger)

    for span in doc.spans:
        span.normalize(morph_vocab)

    for span in doc.spans[:5]:
        if span.type == LOC:
            span.extract_fact(names_extractor)

    names = []

    for s in doc.spans[:5]:
        names.append(s.normal)

    addrs=[]
    addr_extractor = AddrExtractor(morph_vocab)
    m = addr_extractor.find(text)
    if m is not None:
        for addr in m.fact.parts:
            addrs.append(addr.__dict__)

    data_set = {"Names": names, "Addresses": addrs}
    return json.dumps(data_set)




if __name__ == '__main__':
    input = sys.argv[1]
    input = base64.b64decode(input).decode('utf-8')
    result = analise(input)
    result = result.encode("UTF-8")
    print(base64.b64encode(result))




